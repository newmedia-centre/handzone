// import dependencies
import { Server } from 'socket.io'
import { initNamespace } from './namespace'
import { robots } from '@/server/robot'
import { docker } from '@/server/docker'
import { verifyPin } from '@/server/db/pin'
import { generateAccessToken } from '@/server/db/jwt'
import { socketLogger as logger } from '../logger'
import { prisma } from '@/server/db'

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

		// get the otp pin and signature
		const otp = socket.handshake.auth.pin as string
		const signature = socket.handshake.auth.signature as string

		// check if the pin is valid
		verifyPin(otp, signature).then(user => {
			if (!user) return next(new Error('Pin not claimed'))

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

		// join real robot
		socket.on('real', async (callback) => {
			logger.info('Real robot requested', { user: socket.data.user })

			// check if the user has access to a real robot
			const request = await prisma.robotSessionRequest.findFirst({
				where: {
					userId: socket.data.user.id,
					session: {
						start: {
							lte: new Date().toISOString()
						},
						end: {
							gte: new Date().toISOString()
						}
					},
					status: {
						in: ['ACCEPTED']
					}
				},
				include: {
					session: {
						include: {
							robot: true
						}
					}
				}
			})

			// check if the request was found
			if (!request) {
				logger.info('Real robot request denied, no approved session', { user: socket.data.user })
				return callback(false, 'Cound not find an approved scheduled session for the user')
			}

			// get the real robot
			const robot = robots.connections.get(request.session.robot.name)
			if (!robot) {
				logger.info('Real robot request denied, robot not found', { user: socket.data.user })
				return callback(false, 'Robot not found')
			}

			// check if the robot is available
			if (!robot.active) {
				logger.info('Real robot request denied, not yet available', { user: socket.data.user })
				return callback(false, 'Found a scheduled session for the user, but the robot is not yet available')
			}

			// generate the access token
			const token = await generateAccessToken(socket.data.user, robot.info)
			logger.info('Real robot request approved', { user: socket.data.user })
			socket.data.namespace = robot.info
			callback(true, { robot: robot.info, token })
		})

		// send joined namespace
		socket.on('namespace', async (callback) => {
			const found = Array.from(server.sockets.sockets.values()).find(s => s.data.user.id === socket.data.user.id && !!s.data.namespace)
			if (!found || !found.data.namespace) return callback(false, 'No namespace found for user')

			const token = await generateAccessToken(socket.data.user, found.data.namespace)
			callback(true, { robot: found.data.namespace, token })
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
