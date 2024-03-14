// import types
import type { Socket } from 'socket.io'
import type { NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData } from './interface'
import type { PositionDataOut } from './interface/unity'

export const handleUnityEvents = (socket: Socket<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>, positions: Map<string, PositionDataOut>) => {

	// handle the unity:message event
	socket.on('unity:message', (message) => {

		// send the message to all clients
		socket.broadcast.emit('unity:message', (message))
	})

	// handle the unity:position event
	socket.on('unity:position', (payload) => {
		positions.set(socket.id, {
			id: socket.id,
			...payload,
			name: socket.data.name,
			color: socket.data.color
		})
	})

	// handle the unity:pendant event
	socket.on('unity:pendant', (payload) => {
		socket.broadcast.emit('unity:pendant', { owner: socket.id, position: payload })
	})
}