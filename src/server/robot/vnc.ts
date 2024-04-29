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
	_state: RFBHandshake
	_attempts: number

	constructor(robot: RobotInfo) {
		// initialize the EventEmitter
		super()

		// initialize the class variables
		this.state = 'connecting'
		this.serverInit = Buffer.from([])
		this._state = RFBHandshake.CONNECTING
		this._attempts = 5

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
					console.log('vnc data: ', data.length)
					this.emit('data', data)
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
		const pixelFormat = data.subarray(3, 20)
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
				this.socket.write(Buffer.from([0])) // shared flag: 0: exclusive, 1: shared
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
			console.log('RFB INIT')

			// save the server init message
			this.serverInit = data

			// set the state to connected
			this._state = RFBHandshake.CONNECTED
			this.state = 'connected'
			return
		}
	}
}