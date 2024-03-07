// import types
import type { Socket } from 'socket.io'
import type { NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData } from './interface'

export const handleGrasshopperEvents = (socket: Socket<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>) => {
	// handle the unity:message event
	socket.on('unity:message', (message) => {

		// send the json payload to the TCP server
		socket.broadcast.emit('unity:message', (message))
	})
}