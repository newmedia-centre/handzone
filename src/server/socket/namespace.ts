// import dependencies
import { handleInterfacesEvents } from './interfaces'
import { handleInternalsEvents } from './internals'
import { handleRealtimeEvents } from './realtime'
import { handleMotionEvents } from './motion'
import { handleGrasshopperEvents } from './grasshopper'
import { handleUnityEvents } from './unity'

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

		// handle socket disconnection
		socket.on('disconnect', () => {
			// do nothing
		})

		// handle all the incoming events
		handleMotionEvents(socket)
		handleGrasshopperEvents(socket)
		handleRealtimeEvents(socket)
		handleInterfacesEvents(socket)
		handleInternalsEvents(socket)
		handleUnityEvents(socket, positions)

		// handle incoming vnc messages
		socket.on('vnc', (data) => {
			console.log('VNC CLIENT:', Buffer.from(data, 'base64'))
			robot.vnc?.send(Buffer.from(data, 'base64'))
		})

		socket.on('vnc:pixelformat', (data) => {
			robot.vnc?.updatePixelFormat(Buffer.from(data, 'base64'))
			robot.vnc?.send(Buffer.from(data, 'base64'))
		})

		// forward vnc events
		robot.vnc?.on('data', data => {
			console.log('VNC Server:', data)
			socket.emit('vnc', data.toString('base64'))
		})

		console.log('VNC INIT SEND:', robot.vnc?.serverInit)
		socket.emit('vnc:init', robot.vnc?.serverInit.toString('base64') || '')

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