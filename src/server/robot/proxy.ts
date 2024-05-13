// import dependencies
import { Socket, createServer } from 'net'
import env from '../environment'

type RobotInfo = typeof env['ROBOTS'][number]

// import types
import type { Server } from 'net'

export class VNCProxy {
	server: Server
	clients: Set<VNCClient>

	constructor(robot: RobotInfo) {
		this.clients = new Set()
		this.server = createServer(client => {
			console.info(`[ROBOT-VNC:${robot.address}] Client Connected`)

			this.clients.add(new VNCClient(client, robot))
		})

		// start the server on the vnc port
		this.server.listen(env.VNC_PORT, () => {
			console.info(`[ROBOT-VNC:${robot.address}] VNC proxy listening on port ${env.VNC_PORT}`)
		})
	}
}

export class VNCClient {
	_client: Socket
	_vnc?: Socket

	constructor(client: Socket, robot: RobotInfo) {
		this._client = client

		// check if the first message is a valid auth token
		this._client.once('data', message => {
			if (message.toString('utf8') === 'token') {
				console.info(`[ROBOT-VNC:${robot.address}] Authenticated`)

				// connect to robot vnc server
				this._vnc = new Socket()
				this._vnc.setTimeout(5000)
				console.info(`[ROBOT-VNC:${robot.address}] Connecting...`)
				this._vnc.connect(robot.vnc!, robot.address)

				// retry until a connection is established
				this._vnc.on('error', (error: NodeJS.ErrnoException) => {
					// log any errors
					console.error(error)
				})

				this._vnc.on('connect', () => {
					console.info(`[ROBOT-VNC:${robot.address}] Connected`)

					// pipe the sockets to each other
					this._vnc!.pipe(this._client)
					this._client.pipe(this._vnc!)
				})

				this._vnc.on('close', () => {
					console.info(`[ROBOT-VNC:${robot.address}] Robot closed the connection`)
					this._client.destroy()
					this._vnc?.destroy()
				})
			} else {
				console.info(`[ROBOT-VNC:${robot.address}] Authentication failed`)
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
			console.info(`[ROBOT-VNC:${robot.address}] Disconnected`)
			this._client.destroy()
			this._vnc?.destroy()
		})
	}
}