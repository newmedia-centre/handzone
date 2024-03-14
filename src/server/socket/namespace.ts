// import dependencies
import { handleInterfacesEvents } from './interfaces'
import { handleRealtimeEvents } from './realtime'
import { handleMotionEvents } from './motion'
import { handleGrasshopperEvents } from './grasshopper'

// import types
import type { Namespace } from 'socket.io'
import type { NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData } from './interface'
import type { PositionDataOut } from './interface/unity'
import type { RobotConnection } from '../robot'
import { handleUnityEvents } from './unity'

/** Initialize a new namespace by handling all the required events */
export const initNamespace = (namespace: Namespace<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>, robot: RobotConnection) => {

	// create a map to store position data
	const positions = new Map<string, PositionDataOut>()

	// handle the connection to the namespace
	namespace.on('connection', socket => {
		console.log(`Socket ${socket.handshake.address}, ${socket.id} connected to namespace ${robot.info.address}`)

		// add the target to the socket data
		socket.data.robot = robot

		// handle socket disconnection
		socket.on('disconnect', () => {
			// do nothing
		})

		// handle all the incoming events
		handleMotionEvents(socket)
		handleGrasshopperEvents(socket)
		handleRealtimeEvents(socket)
		handleInterfacesEvents(socket)
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
		namespace.emit('unity:position', Array.from(positions.values()))
	}, 200)

	console.log('Namespace initialized:', robot.info.name)

}