// import dependencies
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
	NamespaceSocketData,
	RobotNamespace
} from './interface'
import type { Namespace } from 'socket.io'
import type { RobotSession } from '@/types/Socket/Index'

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
		console.log('Socket User middleware running', socket.handshake.auth, pin.length)

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
		// create the namespace if it doesn't exist
		if (!namespaces.has(robot.info.name)) {
			const nsp = initNamespace((server.of(`/${robot.info.name}`) as unknown) as Namespace<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>, robot)
			namespaces.set(robot.info.name, { robot, nsp })
		}

		// send the new robot list if the robot is virtual
		if (robot.virtual) {
			// filter the namespaces for virtual robots
			const sessions: RobotSession[] = Array.from(namespaces.values()).filter(n => n.robot.virtual).map(n => ({
				name: n.robot.info.name,
				address: n.robot.info.address,
				users: Array.from(n.nsp.sockets.values()).map(s => s.data.user.name ?? '')
			}))

			// send the capacity and sessions
			const capacity = docker.getCapacity()
			server.emit('sessions', { capacity, sessions })
		}
	})

	robots.on('leave', (robot) => {
		// delete the namespace if it exists
		const namespace = namespaces.get(robot.info.name)
		if (namespace) {
			namespace.nsp.disconnectSockets()
			namespace.nsp.removeAllListeners()
			namespaces.delete(robot.info.name)
			server._nsps.delete(`/${robot.info.name}`)
		}

		// send the new robot list if the robot is virtual
		if (robot.virtual) {
			// filter the namespaces for virtual robots
			const sessions: RobotSession[] = Array.from(namespaces.values()).filter(n => n.robot.virtual).map(n => ({
				name: n.robot.info.name,
				address: n.robot.info.address,
				users: Array.from(n.nsp.sockets.values()).map(s => s.data.user.name ?? '')
			}))

			// send the capacity and sessions
			const capacity = docker.getCapacity()
			server.emit('sessions', { capacity, sessions })
		}
	})

	// handle connection events
	server.on('connection', (socket) => {
		console.log(`Socket ${socket.handshake.address}, ${socket.id} connected`)

		// filter the namespaces for virtual robots
		const sessions: RobotSession[] = Array.from(namespaces.values()).filter(n => n.robot.virtual).map(n => ({
			name: n.robot.info.name,
			address: n.robot.info.address,
			users: Array.from(n.nsp.sockets.values()).map(s => s.data.user.name ?? '')
		}))

		// send the capacity and sessions
		const capacity = docker.getCapacity()
		server.emit('sessions', { capacity, sessions })

		// join robot session
		socket.on('join', async (name, callback) => {
			// get the robot
			const robot = robots.connections.get(name)
			if (!robot) return callback(false)

			// generate the access token
			const token = await generateAccessToken(socket.data.user, robot.info)
			socket.data.namespace = robot.info
			callback(true, { robot: robot.info, token })
		})

		// spawn virtual robot
		socket.on('virtual', async (callback) => {
			// try to spawn a virtual robot
			const robot = await docker.requestVirtualRobot()

			// connect to the virtual robot
			console.log('Virtual Robot:', env.DOCKER.OPTIONS.host, robot.Config.Labels['slot'])
			const info = await robots.connectVirtualRobot(robot)

			// generate the access token
			const token = await generateAccessToken(socket.data.user, info)
			socket.data.namespace = info
			callback(true, { robot: info, token })
		})

		// send joined namespace
		socket.on('namespace', async (callback) => {
			if (!socket.data.namespace) return callback(false, 'No namespace found')

			const token = await generateAccessToken(socket.data.user, socket.data.namespace)
			callback(true, { robot: socket.data.namespace, token })
		})
	})

	return server
}

// init socket.io server
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const [io, namespaces]: [Server<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData>, Map<string, RobotNamespace>] = [global.io ?? init(), global.namespaces ?? new Map()]
export default io

// fix global instancing in production // TODO
global.io = io
global.namespaces = namespaces
