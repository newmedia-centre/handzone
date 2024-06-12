// import types
import type { Socket } from 'socket.io'
import type { NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData } from './interface'
import type { PlayerData } from '@/types/Socket/Unity'

export const handleUnityEvents = (socket: Socket<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>, players: Map<string, PlayerData>) => {

	// handle the unity:message event
	socket.on('unity:message', ({ message }) => {

		// send the message to all clients
		socket.broadcast.emit('unity:message', ({ message }))
	})

	// handle the unity:position event
	socket.on('unity:player', ({ hmd, left, right, cursor }) => {
		players.set(socket.id, {
			id: socket.id,
			hmd,
			left,
			right,
			cursor,
			name: socket.data.user.name ?? '',
			color: socket.data.color
		})
	})

	// handle the unity:pendant event
	socket.on('unity:pendant', () => {
		if (socket.data.robot.paused) return
		socket.nsp.emit('unity:pendant', { owner: socket.id })
	})
}