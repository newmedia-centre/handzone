// import types
import type { Socket } from 'socket.io'
import type { ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData } from '../interfaces'

export const handleMotionEvents = (socket: Socket<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData>) => {

	// handle the conveyor_pulse_decode event
	socket.on('motion:conveyor_pulse_decode', (type, a, b) => {
		// forward the command to the tcp stream
	})

}