// import dependencies
import { EventEmitter } from 'events'
import { Socket } from 'net'
import env from '../environment'

// import types
import type { RobotEmitter, TCPEmitter } from './events'

/** The TCP Server for communicating with the robots */
export class TCPServer extends (EventEmitter as new () => TCPEmitter) {
	/** The TCP Sockets for sending messages through the TCP Server */
	connections: Map<string, Socket>

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
		const socket = this.connections.get(address)
		if (!socket) throw new Error(`No write connection found for address ${address}`)

		// send the instruction as a utf-8 buffer
		socket.write(Buffer.from(instruction, 'utf-8'))
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
		const client = new Socket()
		client.setTimeout(5000)
		client.connect(port, address)

		// retry until a connection is established
		client.on('error', (error: NodeJS.ErrnoException) => {
			if (error.code === 'ECONNREFUSED') {
				return setTimeout(() => {
					console.info(`[WRITE:${address}] Retrying...`)
					client.connect(port, address)
				}, client.timeout || 1000)
			}
			console.error(error)
		})

		// add clients when connected
		client.on('connect', () => {
			this.connections.set(address, client)
			this.emit('join', address, this.connections)
			console.info(`[WRITE:${address}] Connected`)
		})

		// remove from clients when closed
		client.on('close', () => {
			if (this.connections.has(address)) {
				this.connections.delete(address)
				this.emit('join', address, this.connections)
				console.info(`[WRITE:${address}] Disconnected`)
			}
		})
	}

}

/** Listens for data from a robot over a TCP socket */
export class ReadConnection extends (EventEmitter as new () => RobotEmitter) {
	/** The TCP socket for reading data */
	socket: Socket

	constructor(socket: Socket, address: string) {
		// initialize the EventEmitter
		super()

		// initialize the class variables
		this.socket = socket

		// handle incoming messages
		this.socket.on('data', (data) => {
			// parse the data
			const message = data.toString()
			console.log('received message:', message, address)

			// emit the message
			this.emit('message', message)
		})

	}
}

export default TCPServer