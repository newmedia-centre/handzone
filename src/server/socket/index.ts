// import dependencies
import { Server } from 'socket.io'
import { initNamespace } from './namespace'
import { robots } from '@/server/robot'
import { docker } from '@/server/docker'
import { validatePin } from '@/server/db/pin'
import { generateAccessToken } from '@/server/db/jwt'
import { socketLogger as logger } from '../logger'

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
import type { RobotConnection } from '../robot/connection'

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
		logger.http(`Incoming connection from ${socket.handshake.address}`)

		// get the pin number
		const pin = socket.handshake.auth.pin as string

		// check if the pin is valid
		validatePin(pin).then(user => {
			// attach the user to the socket
			socket.data.user = user
			return next()
		}).catch(e => {
			logger.warn('Could not authenticate user', { error: e })
			return next(new Error('User not authenticated'))
		})
	})

	// forward read and write events
	robots.on('join', (robot) => {
		// create the namespace if it doesn't exist
		if (!namespaces.has(robot.info.name)) {
			const nsp = initNamespace((server.of(`/${robot.info.name}`) as unknown) as Namespace<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>, robot, logger.child({ entity: 'namespace', robot: robot.info, label: `SOCKET:NSP:${robot.info.name}` }))
			namespaces.set(robot.info.name, { robot, nsp })
		}

		// send the new robot list if the robot is virtual
		if (robot.virtual) {
			// filter the namespaces for virtual robots
			const sessions: RobotSession[] = Array.from(namespaces.values()).filter(n => n.robot.virtual).map(n => ({
				name: n.robot.info.name,
				address: n.robot.info.address,
				type: n.robot.virtual ?? 'sandbox',
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
			logger.info('Removing namespace', { robot: robot.info })
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
				type: n.robot.virtual ?? 'sandbox',
				users: Array.from(n.nsp.sockets.values()).map(s => s.data.user.name ?? '')
			}))

			// send the capacity and sessions
			const capacity = docker.getCapacity()
			server.emit('sessions', { capacity, sessions })
		}
	})

	// handle connection events
	server.on('connection', (socket) => {
		logger.http(`Socket ${socket.id} connected`, { user: socket.data.user })

		// filter the namespaces for virtual robots
		const sessions: RobotSession[] = Array.from(namespaces.values()).filter(n => n.robot.virtual).map(n => ({
			name: n.robot.info.name,
			address: n.robot.info.address,
			type: n.robot.virtual ?? 'sandbox',
			users: Array.from(n.nsp.sockets.values()).map(s => s.data.user.name ?? '')
		}))

		// send the capacity and sessions
		const capacity = docker.getCapacity()
		server.emit('sessions', { capacity, sessions })

		// join robot session
		socket.on('join', async (name, callback) => {
			// get the robot
			const robot = robots.connections.get(name)
			if (!robot) return callback(false, 'Robot not found')

			// generate the access token
			const token = await generateAccessToken(socket.data.user, robot.info)
			socket.data.namespace = robot.info
			callback(true, { robot: robot.info, token })
		})

		// spawn virtual robot
		socket.on('virtual', async (type, callback) => {
			logger.info('Virtual robot requested', { user: socket.data.user })

			// try to spawn a virtual robot
			const robot = await docker.requestVirtualRobot()

			// connect to the virtual robot
			const info = await robots.connectVirtualRobot(robot, type)

			// generate the access token
			const token = await generateAccessToken(socket.data.user, info)
			socket.data.namespace = info

			// wait for the robot to be ready
			await new Promise((resolve) => {
				const listen = (connection: RobotConnection) => {
					if (connection.info.name === info.name) {
						robots.off('join', listen)

						// wait for an additional second to be sure
						setTimeout(() => resolve(true), 1000)
					}
				}
				robots.on('join', listen)
			})

			callback(true, { robot: info, token })
		})

		// send joined namespace
		socket.on('namespace', async (callback) => {
			if (!socket.data.namespace) return callback(false, 'No namespace found for user')

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

global.io = io
global.namespaces = namespaces
