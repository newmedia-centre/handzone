// import types
import type { Socket } from 'socket.io'
import type { ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData } from '../interface'
import type { TCPServer } from '@/server/tcp'

export const handleRTDEEvents = (socket: Socket<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData>, tcp: TCPServer) => {

	// handle the rtde:speed_slider_mask event
	socket.on('rtde:speed_slider_mask', (value) => {
		tcp.send(socket.data.robot, `speed_slider_mask(${value})\n`)
	})

	// forward the robot events
	const robot = tcp.readConnections.get(socket.data.robot)
	if (!robot) return false

	// forward robot messages
	robot.on('message', (message) => {
		socket.emit('message', message)
	})

	return true

}