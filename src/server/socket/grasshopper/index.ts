// import types
import type { Socket } from 'socket.io'
import type { ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData } from '../interface'
import type { TCPServer } from '@/server/tcp'

export const handleGrasshopperEvents = (socket: Socket<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData>, tcp: TCPServer) => {
	// handle the grasshopper:program event
	socket.on('grasshopper:program', (json) => {

		// send the json payload to the TCP server
		socket.broadcast.emit('grasshopper:program', (json));
	})
}