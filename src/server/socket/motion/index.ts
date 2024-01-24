// import types
import type { Socket } from 'socket.io'
import type { ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData } from '../interfaces'
import type { TCPServer } from '@/server/tcp'

export const handleMotionEvents = (socket: Socket<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData>, tcp: TCPServer) => {

	// handle the motion:movej event
	socket.on('motion:movej', (q, a, v, t, r) => {
		tcp.send(socket.data.target.address, `movej(${q}, a=${a}, v=${v}, t=${t}, r=${r})`)
	})

	// handle the motion:speedj event
	socket.on('motion:speedj', (qd, a, t) => {
		tcp.send(socket.data.target.address, `speedj(${qd}, a=${a}, t=${t})`)
	})

	// handle the motion:speedl event
	socket.on('motion:speedl', (xd, a, t, aRot) => {
		tcp.send(socket.data.target.address, `speedl(${xd}, a=${a}, t=${t}, aRot=${aRot})`)
	})

}