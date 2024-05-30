// import dependencies
import { Socket, createServer } from 'net'
import { validateAccessToken } from '../db/jwt'
import env from '../environment'

// import types
import type { Server } from 'net'
import type { RobotManager } from '.'
import type { RobotInfo } from './connection'

export class VNCProxy {
	server: Server
	clients: Set<VNCClient>

	constructor(robots: RobotManager) {
		this.clients = new Set()
		this.server = createServer(client => {
			console.info(`[ROBOT-VNC]: Client Connected`)

			this.clients.add(new VNCClient(client, robots))
		})

		// start the server on the vnc port
		this.server.listen(env.VNC_PORT, () => {
			console.info(`[ROBOT-VNC]: VNC proxy listening on port ${env.VNC_PORT}`)
		})
	}
}

export class VNCClient {
	manager: RobotManager
	robot?: RobotInfo
	_client: Socket
	_vnc?: Socket

	constructor(client: Socket, manager: RobotManager) {
		this.manager = manager
		this._client = client

		// check if the first message is a valid auth token
		this._client.once('data', async (message) => {
			const token = message.toString('utf8')
			const { user, robot } = await validateAccessToken(token)

			if (robot && robot.vnc) {
				console.info(`[ROBOT-VNC]: User ${user.name} Authenticated for robot: ${robot.name}`)

				// connect to robot vnc server
				this._vnc = new Socket()
				this._vnc.setTimeout(5000)
				console.info(`[ROBOT-VNC]: Connecting to robot: ${robot.name}...`)
				this._vnc.connect(robot.vnc, robot.address)

				// retry until a connection is established
				this._vnc.on('error', (error: NodeJS.ErrnoException) => {
					// log any errors
					console.error(error)
				})

				this._vnc.on('connect', () => {
					console.info(`[ROBOT-VNC]: Connected to robot: ${robot.name}...`)

					// pipe the sockets to each other
					this._vnc!.pipe(this._client)
					this._client.pipe(this._vnc!)
				})

				this._vnc.on('close', () => {
					console.info(`[ROBOT-VNC]: Robot closed the connection to robot: ${robot.name}...`)
					this._client.destroy()
					this._vnc?.destroy()
				})

				this.robot = robot
			} else {
				console.info(`[ROBOT-VNC]: Authentication failed`)
				this._client.destroy()
				this._vnc?.destroy()
			}
		})

		this._client.on('error', (error: NodeJS.ErrnoException) => {
			// log any errors
			console.error(error)
		})

		// remove from clients when closed
		this._client.on('close', () => {
			console.info(`[ROBOT-VNC]: Disconnected from robot: ${this.robot?.name}...`)
			this._client.destroy()
			this._vnc?.destroy()
		})
	}
}