// import dependencies
import { EventEmitter } from 'events'
import { Socket } from 'net'
import Semaphore from 'semaphore-async-await'
import { VNCProxy } from './vnc'
import env from '../environment'
import { RobotConnection } from './connection'
import { robotLogger as logger } from '../logger'

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

	/** Tries to connect to an endpoint */
	async connectVirtualRobot(container: ContainerInspectInfo, virtual: SessionType) {
		// get the robot slot
		const slot = container.Config.Labels['slot']
		if (!slot) {
			logger.error('Found container without slot label')
			throw new Error('Found container without slot label')
		}

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
		if (!address) {
			logger.error('Address is undefined while parsing IPv4 address')
			throw new Error('Address is undefined while parsing IPv4 address')
		}

		if (address.includes(':')) {
			return address.replace(/^.*:/, '')
		}

		return address
	}

	/** Starts the TCP Client */
	_tryCreateRobotConnection(robot: RobotInfo, virtual: SessionType | null = null) {
		// create the logger for the robot
		const robotLogger = logger.child({ entity: 'robot', robot, label: `ROBOT:${robot.name}` })

		// create the TCP client
		const socket = new Socket()
		socket.setTimeout(5000)
		robotLogger.info(`Connecting to robot...`)
		socket.connect(robot.port, robot.address)

		// retry until a connection is established
		socket.on('error', (error: NodeJS.ErrnoException) => {
			if (error.code === 'ECONNREFUSED') {
				return setTimeout(() => {
					robotLogger.info(`Connection failed, retrying...`)
					socket.connect(robot.port, robot.address)
				}, socket.timeout || 1000)
			}
			console.error(error)
		})

		// add clients when connected
		socket.on('connect', () => {
			const connection = new RobotConnection(socket, robot, virtual, robotLogger)
			this.connections.set(robot.name, connection)
			this.emit('join', connection, this.connections)
			robotLogger.info(`Connected`)
		})

		// remove from clients when closed
		socket.on('close', () => {
			const connection = this.connections.get(robot.name)
			if (connection) {
				// remove the connection
				this.emit('leave', connection, this.connections)
				connection.clear()
				this.connections.delete(robot.name)

				// try to reconnect
				robotLogger.info(`Disconnected, retrying...`)
				setTimeout(() => {
					this._tryCreateRobotConnection(robot, virtual)
				}, 5000)
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
