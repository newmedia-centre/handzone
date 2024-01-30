// import dependencies
import { EventEmitter } from 'events'
import { Socket } from 'net'
import { parseRealtimeData } from '@/server/socket/realtime'
import env from '../environment'

// import types
import type { RobotEmitter, TCPEmitter } from './events'

/** The TCP Server for communicating with the robots */
export class TCPServer extends (EventEmitter as new () => TCPEmitter) {
	/** The TCP Sockets for sending messages through the TCP Server */
	connections: Map<string, RobotConnection>

	constructor() {
		// initialize the EventEmitter
		super()

		// initialize the class variables
		this.connections = new Map()

		// try to connect to the robots
		const robots = [...(env.ROBOTS || []), ...(env.VIRTUAL_ROBOTS || [])]
		robots.forEach((robot) => {
			this._tryCreateConnection(robot.address, robot.port)
		})
	}

	/** Sends an instruction to the rover */
	async send(address: string, instruction: string) {
		// get the socket
		const robot = this.connections.get(address)
		if (!robot) throw new Error(`No write connection found for address ${address}`)

		// send the instruction as a utf-8 buffer
		robot.socket.write(Buffer.from(instruction, 'utf-8'))
	}

	// private methods

	/** Parses the given RemoteAddress into an IPv4 Address */
	_parseAddress(address?: string) {
		if (!address) throw new Error('Address is undefined')

		if (address.includes(':')) {
			return address.replace(/^.*:/, '')
		}

		return address
	}

	/** Starts the TCP Client */
	_tryCreateConnection(address: string, port: number) {
		// create the TCP client
		const socket = new Socket()
		socket.setTimeout(5000)
		socket.connect(port, address)

		// retry until a connection is established
		socket.on('error', (error: NodeJS.ErrnoException) => {
			if (error.code === 'ECONNREFUSED') {
				return setTimeout(() => {
					console.info(`[ROBOT:${address}] Retrying...`)
					socket.connect(port, address)
				}, socket.timeout || 1000)
			}
			console.error(error)
		})

		// add clients when connected
		socket.on('connect', () => {
			const connection = new RobotConnection(socket)
			this.connections.set(address, connection)
			this.emit('join', address, this.connections)
			console.info(`[ROBOT:${address}] Connected`)
		})

		// remove from clients when closed
		socket.on('close', () => {
			if (this.connections.has(address)) {
				this.connections.get(address)?.clear()
				this.connections.delete(address)
				this.emit('join', address, this.connections)
				console.info(`[ROBOT:${address}] Disconnected`)
			}
		})
	}

}

/** Listens for data from a robot over a TCP socket */
export class RobotConnection extends (EventEmitter as new () => RobotEmitter) {
	/** The TCP socket for reading data */
	socket: Socket
	interval?: NodeJS.Timeout
	realtimeBuffer?: Buffer

	constructor(socket: Socket) {
		// initialize the EventEmitter
		super()

		// initialize the class variables
		this.socket = socket

		// start the interval at 25hz which should be enough for most applications
		this.interval = setInterval(() => this.handleRealtimeData(), 40)

		// handle incoming messages
		this.socket.on('data', (data) => {
			// check if the data is realtime data
			const header = this.getRealtimeHeader(data)

			// check if the buffer length is the length of the realtime buffer
			if (header) {
				this.realtimeBuffer = data
			} else {
				// parse the data
				const message = data.toString('utf-8')
				//console.log('received message:', data.length)

				// emit the message
				this.emit('message', message)
			}
		})

	}

	clear() {
		clearTimeout(this.interval)
	}

	async handleRealtimeData() {
		// get the latest buffer
		const data = this.realtimeBuffer
		if (!data) return

		// emit the raw realtime data
		this.emit('realtime:raw', data)

		// parse the realtime data
		const parsed = await parseRealtimeData(data)
		this.emit('realtime:parsed', parsed)
	}

	getRealtimeHeader(data: Buffer) {
		// verify the size of the package, if it doesn't match the header, return undefined for not being RTDE data
		const size = data.readUInt32BE(0)
		if (size % data.length !== 0) return undefined

		return {
			size
		}
	}

	getRTDEHeader(data: Buffer) {
		// verify the size of the package, if it doesn't match the header, return undefined for not being RTDE data
		const size = data.readUInt16BE(0)
		if (size !== data.length) return undefined

		const type = data.readUInt8(2)
		switch (type) {
			case 86: return {
				size,
				type: 'RTDE_REQUEST_PROTOCOL_VERSION'
			}
			case 118: return {
				size,
				type: 'RTDE_GET_URCONTROL_VERSION'
			}
			case 77: return {
				size,
				type: 'RTDE_TEXT_MESSAGE'
			}
			case 85: return {
				size,
				type: 'RTDE_DATA_PACKAGE'
			}
			case 79: return {
				size,
				type: 'RTDE_CONTROL_PACKAGE_SETUP_OUTPUTS'
			}
			case 73: return {
				size,
				type: 'RTDE_CONTROL_PACKAGE_SETUP_INPUTS'
			}
			case 83: return {
				size,
				type: 'RTDE_CONTROL_PACKAGE_START'
			}
			case 80: return {
				size,
				type: 'RTDE_CONTROL_PACKAGE_PAUSE'
			}
			default: return undefined
		}
	}
}

export default TCPServer