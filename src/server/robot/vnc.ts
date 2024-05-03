// import dependencies
import { EventEmitter } from 'events'
import { Socket } from 'net'

// import types
import { Buffer } from 'buffer'
import type { VNCEmitter } from './events'
import type env from '../environment'

type RobotInfo = typeof env['ROBOTS'][number]

enum RFBHandshake {
	CONNECTING,
	HANDSHAKE_VERSION,
	HANDSHAKE_SECURITY,
	HANDSHAKE_SECURITY_FAILED,
	HANDSHAKE_SECURITY_RESULT,
	INITIALIZE,
	CONNECTED
}

export class VNCConnection extends (EventEmitter as new () => VNCEmitter) {
	socket: Socket
	state: 'connecting' | 'handshake' | 'connected' | 'disconnected' | 'failed'
	protocolVersion?: string
	serverInit: Buffer
	initialFrameBuffer?: Buffer
	_state: RFBHandshake
	_attempts: number
	_buff: Buffer
	_expectedLength: number
	_currentMessageType: number

	constructor(robot: RobotInfo) {
		// initialize the EventEmitter
		super()

		// initialize the class variables
		this.state = 'connecting'
		this.serverInit = Buffer.from([])
		this._state = RFBHandshake.CONNECTING
		this._attempts = 5
		this._buff = Buffer.from([])
		this._expectedLength = 0
		this._currentMessageType = -1

		// create the TCP client
		this.socket = new Socket()
		this.socket.setTimeout(5000)
		console.info(`[ROBOT-VNC:${robot.address}] Connecting...`)
		this.socket.connect(robot.vnc!, robot.address)

		// retry until a connection is established
		this.socket.on('error', (error: NodeJS.ErrnoException) => {
			// retry if the connection was refused
			if (error.code === 'ECONNREFUSED' && this._attempts-- > 0) {
				return setTimeout(() => {
					console.info(`[ROBOT:${robot.address}] Retrying...`)
					this.socket.connect(robot.port, robot.address)
				}, this.socket.timeout || 1000)
			}

			// set the state to failed if the connection was refused too many times
			if (this._attempts <= 0) {
				this.state = 'failed'
				console.info(`[ROBOT-VNC:${robot.address}] Connect Failed`)
			}

			// log any errors
			console.error(error)
		})

		// add clients when connected
		this.socket.on('connect', () => {
			this._state = RFBHandshake.HANDSHAKE_VERSION
			this.state = 'handshake'
			console.info(`[ROBOT-VNC:${robot.address}] Handshake`)

			// emit data when received
			this.socket.on('data', data => {
				if (this._state === RFBHandshake.CONNECTED) {
					this._buff = Buffer.concat([this._buff, data])
					this._setHeader(data)

					// check if the buffer is complete
					if (this._checkLength()) {
						console.log('SENDING FULL PACKET:', this._buff.length, this._expectedLength)
						this.emit('data', this._buff)
						if (this._currentMessageType === 0 && !this.initialFrameBuffer) {
							this.initialFrameBuffer = Buffer.from(this._buff)
							console.log('INITIAL FRAMEBUFFER SET:', this.initialFrameBuffer.length)
						}
						this._buff = Buffer.from([])
						this._expectedLength = -1
					} else {
						console.log('DATA IS PARTIAL PACKET:', this._buff.length, this._expectedLength)
					}
				} else {
					this._handshake(data)
				}
			})
		})

		// remove from clients when closed
		this.socket.on('close', () => {
			this.state = 'disconnected'
			console.info(`[ROBOT-VNC:${robot.address}] Disconnected`)
		})
	}

	// send data to the vnc server
	send(data: Buffer) {
		this.socket.write(data)
	}

	// update the pixel format
	updatePixelFormat(data: Buffer) {
		const prepend = this.serverInit.subarray(0, 4)
		const pixelFormat = data.subarray(3, 20)
		const append = this.serverInit.subarray(20, this.serverInit.length)
		this.serverInit = Buffer.concat([prepend, pixelFormat, append])
	}

	// handle the vnc handshake
	_handshake(data: Buffer) {
		// protocol version handshake
		if ((this._state === RFBHandshake.HANDSHAKE_VERSION) && (data.length >= 12)) {
			// parse the protocol version
			const protocolVersion = data.subarray(0, 12).toString('ascii')
			console.log('RFB VER: ' + protocolVersion)
			this.protocolVersion = protocolVersion

			// write the protocol version we support
			this._state = RFBHandshake.HANDSHAKE_SECURITY
			this.socket.write(Buffer.from('RFB 003.008\n', 'ascii'))
			return
		}

		// security handshake
		if (this._state === RFBHandshake.HANDSHAKE_SECURITY) {
			// parse the security types supported by the server
			const numberOfSecurityTypes = data.readUInt8()
			const securityTypes = new Array(numberOfSecurityTypes).fill(null).map((_, i) => data.readUInt8(i + 1))
			console.log('SEC VER: ', numberOfSecurityTypes, ': ', securityTypes)

			// check if the number of security types is zero
			if (numberOfSecurityTypes === 0) {
				this._state = RFBHandshake.HANDSHAKE_SECURITY_FAILED
				return
			}

			// check if the 'None' security type is supported
			if (securityTypes.includes(1)) {
				// write
				this._state = RFBHandshake.HANDSHAKE_SECURITY_RESULT
				this.socket.write(Buffer.from([1]))
				return
			}

			// security type not supported by this client
			this.state = 'failed'
			this.socket.end()
			return
		}

		// security result handshake
		if (this._state === RFBHandshake.HANDSHAKE_SECURITY_RESULT) {
			// parse the security result
			const securityResult = data.readUInt32BE()
			console.log('SEC RES: ', securityResult ? 'FAILED' : 'OK')

			// check if the security result is successful
			if (securityResult === 0) {
				this._state = RFBHandshake.INITIALIZE
				this.socket.write(Buffer.from([1])) // shared flag: 0: exclusive, 1: shared
				return
			}

			// security result not successful
			this._state = RFBHandshake.HANDSHAKE_SECURITY_FAILED
			return
		}

		// security failed
		if (this._state === RFBHandshake.HANDSHAKE_SECURITY_FAILED) {
			// parse the reason for the security failure
			const reasonLength = data.readUInt32BE()
			const reasonString = data.subarray(4, 4 + reasonLength).toString('ascii')
			console.log('SEC FAIL: ', reasonLength, ': ', reasonString)

			this.state = 'failed'
			this.socket.end()
			return
		}

		// server init
		if (this._state === RFBHandshake.INITIALIZE) {
			// save the server init message
			console.log('RFB INIT:', data)
			this.serverInit = Buffer.from(data)

			// send pixelformat and encoding
			this.socket.write(Buffer.from([0x00, 0x00, 0x00, 0x00, 0x20, 0x18, 0x00, 0x01, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff, 0x10, 0x08, 0x00, 0x00, 0x00, 0x00]))
			this.socket.write(Buffer.from([0x02, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x00]))
			this.socket.write(Buffer.from([0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x03, 0x20, 0x02, 0x58]))

			setInterval(() => {
				this.socket.write(Buffer.from([0x03, 0x01, 0x00, 0x00, 0x00, 0x00, 0x03, 0x20, 0x02, 0x58]))
			}, 40)

			// set the state to connected
			this._state = RFBHandshake.CONNECTED
			this.state = 'connected'
			return
		}
	}

	// check if the data is a header
	_setHeader(data: Buffer): boolean {
		const messageType = data.readInt8(0)

		if (messageType === 0 || messageType === 1) {
			// check if the bit after the message type is empty padding
			const padding = data.readUInt8(1)

			if (padding === 0) {
				this._currentMessageType = messageType
				console.log('GOT MESSAGE TYPE:', messageType ? 'SET COLOUR MAP' : 'FRAMEBUFFER UPDATE')
				return true
			}
			return false
		}

		// message-type 2 is always a new packet
		if (messageType === 2) {
			this._currentMessageType = messageType
			console.log('GOT MESSAGE TYPE: BELL')
			return true
		}

		if (messageType === 3) {
			// check if the bit after the message type is empty padding
			const padding = data.readUInt8(1) + data.readUInt8(2) + data.readUInt8(3)

			if (padding === 0) {
				this._currentMessageType = messageType
				console.log('GOT MESSAGE TYPE: SERVER CUT TEXT')
				return true
			}
			return false
		}

		return false
	}

	// check message length, return true if the data contains a full message
	_checkLength(): boolean {
		switch (this._currentMessageType) {
			// framebuffer update
			case 0:
				this._expectedLength = 4
				const numRectangles = this._buff.readUInt16BE(2)
				console.log('NUM RECTANGLES:', numRectangles)
				for (let i = 0; i < numRectangles; i++) {
					const encoding = this._buff.readUInt32BE(this._expectedLength + 8)
					this._expectedLength = this._expectedLength + this._getEncodingLength(encoding) + 12
				}
				break
			// set colour map entries
			case 1:
				this._expectedLength = this._buff.readUint16BE(4) + 6
				break
			// bell
			case 2:
				return true
			// server cut text
			case 3:
				this._expectedLength = this._buff.readUInt32BE(4) + 8
				break
		}

		// this might fail cause sometimes we get 2 packets combined, we need to check if this has to do with the errors on the client
		if (this._expectedLength === this._buff.length) {
			return true
		} else {
			return false
		}
	}

	// parse the length for each encoding
	_getEncodingLength(encoding: number): number {
		switch (encoding) {
			case 0:
				return 800 * 600 * 3
			case 1:
				return 4
			case 2:
				const rectangles = this._buff.readUInt32BE(this._expectedLength + 12)
				return 7 + (rectangles * 11)
			case 16:
				const length = this._buff.readUInt32BE(this._expectedLength + 12)
				return length + 4
		}

		console.log('ENC:', encoding, 'NOT SUPPORTED')
		return -1
	}
}