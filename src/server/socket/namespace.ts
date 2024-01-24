// import dependencies
import { handleRTDEEvents } from './rtde'
import { handleMotionEvents } from './motion'

// import types
import type { Namespace } from 'socket.io'
import type { ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData } from './interfaces'
import type TCPServer from '../tcp'

/** Initialize a new namespace by handling all the required events */
export const initNamespace = (namespace: Namespace<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData>, target: SocketData['target'], tcp: TCPServer) => {

	// handle the connection to the namespace
	namespace.on('connection', socket => {
		console.log(`Socket ${socket.id} connected to namespace ${target.address}`)

		// add the target to the socket data
		socket.data.target = target

		// handle socket disconnection
		socket.on('disconnect', () => {
			// do nothing
		})

		// handle all the incoming events
		handleRTDEEvents(socket, tcp)
		handleMotionEvents(socket, tcp)

		// forward events between sockets

		// handle the simulation event
		socket.on('simulation', () => {
			socket.emit('simulation')
		})

		// handle the message event
		socket.on('message', (message) => {
			socket.emit('message', message)
		})

	})

}