// import dependencies
import { handleInterfacesEvents } from './interfaces'
import { handleInternalsEvents } from './internals'
import { handleRealtimeEvents } from './realtime'
import { handleMotionEvents } from './motion'
import { handleGrasshopperEvents } from './grasshopper'
import { handleUnityEvents } from './unity'
import { validateAccessToken } from '@/server/db/jwt'

// import types
import type { Namespace } from 'socket.io'
import type { NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData } from './interface'
import type { PlayerData } from '@/types/Socket/Unity'
import type { RobotConnection } from '../robot/connection'
import type { Logger } from 'winston'

/** Initialize a new namespace by handling all the required events */
export const initNamespace = (namespace: Namespace<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>, robot: RobotConnection, logger: Logger) => {

	// set up the namespace middleware
	namespace.use((socket, next) => {
		// get the token
		const token = socket.handshake.auth.token as string

		// check if the pin is valid
		validateAccessToken(token).then(({ user }) => {
			// attach the user to the socket
			socket.data.user = user
			socket.data.robot = robot
			socket.data.color = Math.floor(Math.random() * 2e24).toString(16)

			return next()
		}).catch(e => {
			logger.warn('Could not authenticate user', { error: e })
			return next(new Error('User not authenticated'))
		})
	})

	// create a map to store player data
	const players = new Map<string, PlayerData>()

	// handle the connection to the namespace
	namespace.on('connection', socket => {
		logger.http(`Socket ${socket.id} connected`, { user: socket.data.user })

		// handle socket disconnection
		socket.on('disconnect', () => {
			// do nothing
		})

		// handle all the incoming events
		handleMotionEvents(socket)
		handleGrasshopperEvents(socket, logger)
		handleRealtimeEvents(socket)
		handleInterfacesEvents(socket)
		handleInternalsEvents(socket)
		handleUnityEvents(socket, players)

		// forward video events
		robot.video?.forEach(video => {
			video.on('frame', (data) => {
				socket.emit('video', video.camera.name, data.toString('base64'))
			})
		})

		// forward events between sockets

		// handle the message event
		socket.on('message', (message) => {
			socket.broadcast.emit('message', message)
		})
	})

	// emit the positions data
	setInterval(() => {
		namespace.emit('unity:players', { players: Array.from(players.values()) })
	}, 200)

	logger.info('Namespace initialized')
	return namespace
}