// import dependencies
import { EventEmitter } from 'events'
import { Socket, createServer } from 'net'
import { Buffer } from 'buffer'
import Semaphore from 'semaphore-async-await'
import { VNCProxy } from './proxy'
import env from '../environment'
import { RobotConnection } from './connection'

// import types
import type { ManagerEmitter } from './events'
import type { ContainerInspectInfo } from 'dockerode'
import type { SessionType } from '@/types/Socket/Index'
import type { RobotInfo } from './connection'

/** The TCP Server for communicating with the robots */
export class RobotManager extends (EventEmitter as new () => ManagerEmitter) {
	/** The TCP Sockets for sending messages through the TCP Server */
	connections: Map<string, RobotConnection>
	vnc: VNCProxy
	_semaphore: Semaphore

	constructor() {
		// initialize the EventEmitter
		super()

		// initialize the class variables
		this.connections = new Map()
		this.vnc = new VNCProxy(this)
		this._semaphore = new Semaphore(1)

		// try to connect to the robots
		env.ROBOTS.forEach((robot) => {
			this._tryCreateRobotConnection(robot)
		})
	}

	/** sends an instruction with a callback */
	async sendCallback(robot: RobotConnection, instruction: string) {
		// acquire a semaphore
		await this._semaphore.acquire()

		const promise = new Promise<Buffer>((resolve, reject) => {
			// set timeout to 5 seconds
			const timeout = setTimeout(() => {
				server.close()
				server.on('close', () => { this._semaphore.release() })

				reject('timeout')
			}, 5000)

			// create a tcp server receive values from the robot and listen on port 4000
			const server = createServer(socket => {

				socket.once('data', data => {
					clearTimeout(timeout)
					server.close()
					server.on('close', () => { this._semaphore.release() })
					resolve(data)
				})

				socket.on('error', () => { })
			})
			server.maxConnections = 1
			server.listen(4000)
		})

		// send the instruction as a utf-8 buffer
		robot.socket.write(Buffer.from(instruction, 'utf-8'))

		return promise
	}

	/** Tries to connect to an endpoint */
	async connectVirtualRobot(container: ContainerInspectInfo, virtual: SessionType) {
		// get the robot slot
		const slot = container.Config.Labels['slot']
		if (!slot) throw new Error('Found container without slot label')

		const port = parseInt(`3${slot}03`)
		const vnc = parseInt(`59${slot}`)

		const info: RobotInfo = {
			name: container.Name.split('/')[1]!,
			address: env.DOCKER.OPTIONS.host,
			port,
			vnc,
			camera: []
		}

		this._tryCreateRobotConnection(info, virtual)
		return info
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
	_tryCreateRobotConnection(robot: RobotInfo, virtual: SessionType | null = null) {
		// create the TCP client
		const socket = new Socket()
		socket.setTimeout(5000)
		console.info(`[ROBOT:${robot.name}] Connecting...`)
		socket.connect(robot.port, robot.address)

		// retry until a connection is established
		socket.on('error', (error: NodeJS.ErrnoException) => {
			if (error.code === 'ECONNREFUSED') {
				return setTimeout(() => {
					console.info(`[ROBOT:${robot.name}] Retrying...`)
					socket.connect(robot.port, robot.address)
				}, socket.timeout || 1000)
			}
			console.error(error)
		})

		// add clients when connected
		socket.on('connect', () => {
			const connection = new RobotConnection(socket, robot, virtual)
			this.connections.set(robot.name, connection)
			this.emit('join', connection, this.connections)
			console.info(`[ROBOT:${robot.name}] Connected`)
		})

		// remove from clients when closed
		socket.on('close', () => {
			if (this.connections.has(robot.name)) {
				const connection = this.connections.get(robot.name)
				this.emit('leave', connection!, this.connections)
				connection?.clear()
				this.connections.delete(robot.name)
				console.info(`[ROBOT:${robot.name}] Disconnected`)
			}
		})
	}

}

// init tcp server
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const robots: RobotManager = global.robots ?? new RobotManager()
export default robots

// fix global instancing in production // TODO
global.robots = robots
