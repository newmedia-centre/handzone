// import types
import type { Socket } from 'socket.io'
import type { ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData } from '../interface'
import type { TCPServer } from '@/server/tcp'

export const handleMotionEvents = (socket: Socket<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData>, tcp: TCPServer) => {

	// handle the motion:movej event
	socket.on('motion:movej', (q, a, v, t, r) => {
		tcp.send(socket.data.robot, `movej([${q}],a=${a},v=${v},t=${t},r=${r})\n`)
	})

	// handle the motion:speedj event
	socket.on('motion:speedj', (qd, a, t) => {
		tcp.send(socket.data.robot, `speedj([${qd}],a=${a},t=${t})\n`)
	})

	// handle the motion:speedl event
	socket.on('motion:speedl', (xd, a, t, aRot) => {
		tcp.send(socket.data.robot, `speedl([${xd}],a=${a},t=${t},aRot=${aRot})\n`)
	})

	// handle the motion:set_pos event
	socket.on('motion:set_pos', (q) => {
		tcp.send(socket.data.robot, `set_pos([${q}])\n`)
	})

}