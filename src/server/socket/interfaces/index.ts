// import types
import type { Socket } from 'socket.io'
import type { NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData } from '../interface'
import type { TCPServer } from '@/server/tcp'

export const handleInterfacesEvents = (socket: Socket<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>, tcp: TCPServer) => {

	// handle the interfaces:set_tool_digital_out event
	socket.on('interfaces:set_tool_digital_out', (n, b) => {
		// make sure to use exactly 'True' or 'False' as bool value.
		const capitalB = String(b).charAt(0).toUpperCase() + String(b).slice(1)
		tcp.send(socket.data.robot, `set_tool_digital_out(${n},${capitalB})\n`)
	})

	socket.on('interfaces:set_standard_digital_out', (n, b) => {
		// make sure to use exactly 'True' or 'False' as bool value.
		const capitalB = String(b).charAt(0).toUpperCase() + String(b).slice(1)
		tcp.send(socket.data.robot, `set_standard_digital_out(${n},${capitalB})\n`)
	})

	socket.on('interfaces:get_inverse_kin', (x, qnear, maxPositionError, tcp_offset) => {
		tcp.send(socket.data.robot, `get_inverse_kin(p[${x}], ${qnear ? `qnear=${qnear}` : ''}, 
		${maxPositionError ? `maxPositionError=${maxPositionError}` : ''}, 
		${tcp_offset ? `tcp=${tcp_offset}` : ''})\n`)
	})
}