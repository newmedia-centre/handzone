// import types
import type { Socket } from 'socket.io'
import type { ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData } from '../interface'
import type { TCPServer } from '@/server/tcp'

export const handleInterfacesEvents = (socket: Socket<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData>, tcp: TCPServer) => {

	// handle the interfaces:set_tool_digital_out event
	socket.on('interfaces:set_tool_digital_out', (n, b) => {
		console.log(`set_tool_digital_out(${n},${b})`)
		tcp.send(socket.data.robot, `set_tool_digital_out(${n},${b})\n`)
	})

	socket.on('interfaces:set_standard_digital_out', (n, b) => {
		console.log(`set_standard_digital_out(${n},${b})`)
		tcp.send(socket.data.robot, `set_standard_digital_out(${n},${b})\n`)
	})


}