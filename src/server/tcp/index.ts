// import dependencies
import { EventEmitter } from 'events'
import { Server, Socket } from 'net'

// import types
import type { RobotEmitter, TCPEmitter } from './events'

/** The TCP Server for communicating with the robots */
export class TCPServer extends (EventEmitter as new () => TCPEmitter) {
	/** The TCP Server for handling incoming messages */
	server: Server
	/** The Emitters for received messages from the TCP Server */
	readConnections: Map<string, ReadConnection>
	/** The TCP Sockets for sending messages through the TCP Server */
	writeConnections: Map<string, Socket>

	constructor(readPort: number, writePort: number, callback: () => void) {
		// initialize the EventEmitter
		super()

		// initialize the class variables
		this.readConnections = new Map()
		this.writeConnections = new Map()

		// create the TCP server
		this.server = new Server((socket) => {

			// add this socket to the read connections
			const address = this._parseAddress(socket.remoteAddress)
			this.readConnections.set(address, new ReadConnection(socket, address))
			this.emit('read', this.readConnections)
			console.info(`[READ:${address}] Connected`)

			// create a new Write connection
			this._tryCreateWriteConnection(address, writePort)

			// handle connection errors
			socket.on('error', (error: NodeJS.ErrnoException) => {
				if (error.code === 'ECONNRESET') return
				console.error(error)
			})

			// handle disconnections
			socket.on('close', () => {
				this.readConnections.delete(address)
				console.info(`[READ:${address}] Disconnected`)
			})

		})

		// listen on the port
		this.server.listen(readPort, () => {
			callback()
		})
	}

	/** Sends an instruction to the rover */
	async send(address: string, instruction: string) {
		// get the socket
		const socket = this.writeConnections.get(address)
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
	_tryCreateWriteConnection(address: string, port: number) {
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
			this.writeConnections.set(address, client)
			this.emit('write', this.writeConnections)
			console.info(`[WRITE:${address}] Connected`)
		})

		// remove from clients when closed
		client.on('close', () => {
			if (this.writeConnections.has(address)) {
				console.info(`[WRITE:${address}] Disconnected`)
				this.writeConnections.delete(address)
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