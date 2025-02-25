/*
 *The MIT License (MIT)
 * Copyright (c) 2025 NewMedia Centre - Delft University of Technology
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

// import dependencies
import { Socket, createServer } from 'net'
import { validateAccessToken } from '../db/jwt'
import { robotLogger } from '../logger'
import env from '../environment'

// import types
import type { Server } from 'net'
import type { RobotManager } from '.'
import type { RobotInfo } from './connection'
import type { Logger } from 'winston'

// create the vnc proxy manager
export class VNCProxy {
	server: Server
	clients: Set<VNCClient>

	constructor(robots: RobotManager) {
		// create logger
		const logger = robotLogger.child({ entity: 'vnc', label: 'ROBOTS:VNC' })

		this.clients = new Set()
		this.server = createServer(client => {
			logger.info(`Client Connected`)

			this.clients.add(new VNCClient(client, robots, logger))
		})

		// start the server on the vnc port
		this.server.listen(env.VNC_PORT, () => {
			logger.info(`VNC proxy listening on port ${env.VNC_PORT}`)
		})
	}
}

// create the vnc proxy client
export class VNCClient {
	manager: RobotManager
	robot?: RobotInfo
	_client: Socket
	_vnc?: Socket
	_logger?: Logger

	constructor(client: Socket, manager: RobotManager, vncLogger: Logger) {
		this.manager = manager
		this._client = client

		// check if the first message is a valid auth token
		this._client.once('data', async (message) => {
			const token = message.toString('utf8')
			try {
				const { robot } = await validateAccessToken(token)

				if (robot.vnc) {
					// create a logger for the robot vnc proxy
					this._logger = vncLogger.child({ entity: 'vnc', label: `ROBOT:VNC:${robot.name}`, robot })

					this._logger.info('User Authenticated for robot')

					// connect to robot vnc server
					this._vnc = new Socket()
					this._vnc.setTimeout(5000)
					this._logger.info('Connecting to robot...')
					this._vnc.connect(robot.vnc, robot.address)

					// retry until a connection is established
					this._vnc.on('error', (error: NodeJS.ErrnoException) => {
						// log any errors
						this._logger?.error(error.message, { error })
					})

					this._vnc.on('connect', () => {
						this._logger?.info('Connected to robot')

						// pipe the sockets to each other
						this._vnc!.pipe(this._client)
						this._client.pipe(this._vnc!)
					})

					this._vnc.on('close', () => {
						this._logger?.info('Robot closed the connection')
						this._client.destroy()
						this._vnc?.destroy()
					})

					this.robot = robot
				} else {
					vncLogger.info('VNC requested for robot without VNC support', { token })
					this._client.destroy()
					this._vnc?.destroy()
				}
			} catch (error) {
				vncLogger.info('Authentication failed', { token, error })
				this._client.destroy()
				this._vnc?.destroy()
			}
		})

		this._client.on('error', (error: NodeJS.ErrnoException) => {
			// log any errors
			this._logger?.error(error.message, { error })
		})

		// remove from clients when closed
		this._client.on('close', () => {
			this._logger?.info('Disconnected from robot')
			this._client.destroy()
			this._vnc?.destroy()
		})
	}
}