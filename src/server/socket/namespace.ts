// import dependencies
import { handleInterfacesEvents } from './interfaces'
import { handleInternalsEvents } from './internals'
import { handleRealtimeEvents } from './realtime'
import { handleMotionEvents } from './motion'
import { handleGrasshopperEvents } from './grasshopper'
import { handleUnityEvents } from './unity'
import { generateAccessToken } from '../db/jwt'

// import types
import type { Namespace } from 'socket.io'
import type { NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData } from './interface'
import type { PlayerData } from '@/types/Socket/Unity'
import type { RobotConnection } from '../robot'

/** Initialize a new namespace by handling all the required events */
export const initNamespace = (namespace: Namespace<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>, robot: RobotConnection) => {

	// create a map to store position data
	const positions = new Map<string, PlayerData>()

	// handle the connection to the namespace
	namespace.on('connection', socket => {
		console.log(`Socket ${socket.handshake.address}, ${socket.id} connected to namespace ${robot.info.address}`)

		// add the target to the socket data
		socket.data.robot = robot
		socket.data.user = {
			id: '1',
			name: 'test',
			email: ''
		}

		// handle socket disconnection
		socket.on('disconnect', () => {
			// do nothing
		})

		// send a token for vnc
		const token = async () => {
			const token = await generateAccessToken(socket.data.user, socket.data.robot.info)
			socket.emit('token', token)
			console.log(`[ROBOT-${socket.data.robot.info.name}]: Token sent to user ${socket.data.user.name}`)
		}
		token()

		// handle all the incoming events
		handleMotionEvents(socket)
		handleGrasshopperEvents(socket)
		handleRealtimeEvents(socket)
		handleInterfacesEvents(socket)
		handleInternalsEvents(socket)
		handleUnityEvents(socket, positions)

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
		namespace.emit('unity:players', { players: Array.from(positions.values()) })
	}, 200)

	console.log('Namespace initialized:', robot.info.name)

}