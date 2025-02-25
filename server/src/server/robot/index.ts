/*
 * Copyright (c) 2024 NewMedia Centre - Delft University of Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the 'License');
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an 'AS IS' BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// import dependencies
import { EventEmitter } from 'events'
import { Socket } from 'net'
import { Sema } from 'async-sema'
import { VNCProxy } from './vnc'
import env from '../environment'
import { RobotConnection } from './connection'
import { robotLogger as logger } from '../logger'
import { prisma } from '@/server/db'

// import types
import type { ManagerEmitter } from './emitter'
import type { ContainerInspectInfo } from 'dockerode'
import type { SessionType } from '@/types/Socket/Index'
import type { RobotInfo } from './connection'

/** The TCP Server for communicating with the robots */
export class RobotManager extends (EventEmitter as new () => ManagerEmitter) {
	/** The TCP Sockets for sending messages through the TCP Server */
	connections: Map<string, RobotConnection>
	vnc: VNCProxy
	_semaphore: Sema

	constructor() {
		// initialize the EventEmitter
		super()

		// initialize the class variables
		this.connections = new Map()
		this.vnc = new VNCProxy(this)
		this._semaphore = new Sema(1)

		// sync the robots with the database
		this._syncRobots().then(robots => {
			robots.forEach(robot => {
				// try to connect to the robots
				robot && this._tryCreateRobotConnection(robot)
			})
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

		let stable: NodeJS.Timeout

		// retry until a connection is established
		socket.on('error', (error: NodeJS.ErrnoException) => {
			if (error.code === 'ECONNREFUSED') {
				return setTimeout(() => {
					robotLogger.http(`Connection failed, retrying...`)
					socket.connect(robot.port, robot.address)
				}, 60000)
			}
			console.error(error)
		})

		// add clients when connected
		socket.on('connect', () => {
			const connection = new RobotConnection(socket, robot, virtual, robotLogger)
			this.connections.set(robot.name, connection)
			stable = setTimeout(() => {
				this.emit('join', connection, this.connections)
			}, 10000)
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
				clearTimeout(stable)
				setTimeout(() => {
					this._tryCreateRobotConnection(robot, virtual)
				}, 5000)
			}
		})
	}

	/** Syncs the robots with the database */
	async _syncRobots() {
		// disable all robots
		await prisma.robot.updateMany({
			data: {
				active: false
			}
		})

		// iterate through the env robots
		const tasks = env.ROBOTS.map(async (robot) => {
			try {

				// find the robot in the database
				const updated = await prisma.robot.update({
					where: {
						name: robot.name
					},
					data: {
						active: true
					}
				}).catch(error => {
					// check if the robot was not found
					if (error.code === 'P2025') {
						logger.info(`Robot ${robot.name} not found in the database, creating...`, { robot })
					} else {
						throw error
					}
				})

				if (!updated) {
					// create the robot in the database
					await prisma.robot.create({
						data: {
							name: robot.name,
							type: 'URSIM_CB3_3_15_8',
							active: true
						}
					})

					logger.info(`Inserted robot ${robot.name} from the env into the database`, { robot })
				} else {
					logger.info(`Activated robot ${robot.name}`, { robot })
				}

				return robot
			} catch (error) {
				logger.error(`Error syncing robot ${robot.name} with the database`, { error, robot })
			}
		})

		// wait for all tasks to complete
		logger.info('Syncing robots with the database...')
		const res = await Promise.all(tasks)
		logger.info('Synced robots with the database!')

		return res
	}
}

// init tcp server
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const robots: RobotManager = global.robots ?? new RobotManager()
export default robots

global.robots = robots
