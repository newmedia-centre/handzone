// import types
import type { Socket } from 'socket.io'
import type { NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData } from '../interface'
import type { TCPServer } from '@/server/tcp'

export const handleGrasshopperEvents = (socket: Socket<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>, tcp: TCPServer) => {
	// handle the grasshopper:program event
	socket.on('grasshopper:program', (json) => {

		// send the json payload to the TCP server
		socket.broadcast.emit('grasshopper:program', (json));
	})
}