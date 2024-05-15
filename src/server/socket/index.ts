// import dependencies
import type { Namespace } from 'socket.io'
import { Server } from 'socket.io'
import { initNamespace } from './namespace'
import { robots } from '@/server/robot'
import { docker } from '@/server/docker'
import { validatePin } from '@/server/db/pin'
import { generateAccessToken } from '@/server/db/jwt'
import { env } from '@/server/environment'

// import types
import type {
	ClientToServerEvents,
	ServerToClientEvents,
	InterServerEvents,
	SocketData,
	NamespaceClientToServerEvents,
	NamespaceServerToClientEvents,
	NamespaceSocketData
} from './interface'

// create socket.io server
export const init = () => {
	// create server instance
	const server = new Server<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData>({
		serveClient: true,
		maxHttpBufferSize: 1e8,
		cors: {
			origin: true,
			methods: ['GET', 'POST'],
			credentials: true
		}
	})

	// set up the index server middleware
	server.use((socket, next) => {
		// get the pin number
		const pin = socket.handshake.auth.pin as string

		// check if the pin is valid
		validatePin(pin).then(user => {
			// attach the user to the socket
			socket.data.user = user
			return next()
		}).catch(e => {
			console.log('Could not authenticate user', e)
			return next(new Error('User not authenticated'))
		})
	})

	// forward read and write events
	robots.on('join', (robot) => {
		server.emit('robots', { real: null, sessions: [] })

		// create the namespace if it doesn't exist
		server._nsps.has(`/${robot.info.name}`) || initNamespace((server.of(`/${robot.info.name}`) as unknown) as Namespace<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>, robot)
	})

	robots.on('leave', (address) => {
		server.emit('robots', { real: null, sessions: [] })

		// delete the namespace if it exists
		server._nsps.has(`/${address}`) && server._nsps.delete(`/${address}`)
	})

	// handle connection events
	server.on('connection', (socket) => {
		console.log(`Socket ${socket.handshake.address}, ${socket.id} connected`)
		socket.emit('robots', { real: null, sessions: [] })

		// join robot session
		socket.on('join', async (name, callback) => {
			// get the robot
			const robot = robots.connections.get(name)
			if (!robot) return callback(false)

			// generate the access token
			const token = await generateAccessToken(socket.data.user, robot.info)
			callback(true, { robot, token })
		})

		// spawn virtual robot
		socket.on('virtual', async (callback) => {
			// try to spawn a virtual robot
			const robot = await docker.requestVirtualRobot()

			// connect to the virtual robot
			console.log('Virtual Robot:', env.DOCKER.OPTIONS.host, '30103')
			const info = await robots.connectVirtualRobot(robot, 30103, 5901)

			// generate the access token
			const token = await generateAccessToken(socket.data.user, info)
			callback(true, { robot, token })
		})
	})

	return server
}

// init socket.io server
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const io: Server<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData> = global.io ?? init()
export default io

// fix global instancing in production // TODO
global.io = io
