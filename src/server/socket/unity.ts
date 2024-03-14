// import types
import type { Socket } from 'socket.io'
import type { NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData } from './interface'
import type { PlayerData } from './interface/unity'

export const handleUnityEvents = (socket: Socket<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>, players: Map<string, PlayerData>) => {

	// handle the unity:message event
	socket.on('unity:message', ({ message }) => {

		// send the message to all clients
		socket.broadcast.emit('unity:message', ({ message }))
	})

	// handle the unity:position event
	socket.on('unity:player', ({ hmd, left, right }) => {
		players.set(socket.id, {
			id: socket.id,
			hmd,
			left,
			right,
			name: socket.data.name,
			color: socket.data.color
		})
	})

	// handle the unity:pendant event
	socket.on('unity:pendant', ({ position, rotation }) => {
		socket.broadcast.emit('unity:pendant', { owner: socket.id, position, rotation })
	})
}