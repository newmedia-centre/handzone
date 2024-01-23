// import types
import type { Namespace } from 'socket.io'
import type { ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData } from './interfaces'

export const initNamespace = (namespace: Namespace<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData>, target: SocketData['target']) => {

	// handle the connection to the namespace
	namespace.on('connection', socket => {

		// add the target to the socket data
		socket.data.target = target

		// handle socket disconnection
		socket.on('disconnect', () => {
			// do nothing
		})

	})

}