// import types
import type { Socket } from 'socket.io'
import type { ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData } from '../interfaces'
import type { TCPServer } from '@/server/tcp'

export const handleMotionEvents = (socket: Socket<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData>, tcp: TCPServer) => {

	// handle the interfaces:set_tool_digital_out event
	socket.on('interfaces:set_tool_digital_out', (n, b) => {
		tcp.send(socket.data.target.address, `set_tool_digital_out(${n}, ${b})`)
	})

}