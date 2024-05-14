// import dependencies
import { robots } from '@/server/robot'

// import types
import type { Socket } from 'socket.io'
import type { NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData } from './interface'

export const handleInterfacesEvents = (socket: Socket<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>) => {

	// handle the interfaces:set_tool_digital_out event
	socket.on('interfaces:set_tool_digital_out', ({ n, b }) => {
		// make sure to use exactly 'True' or 'False' as bool value.
		const capitalB = String(b).charAt(0).toUpperCase() + String(b).slice(1)
		robots.send(socket.data.robot, `set_tool_digital_out(${n},${capitalB})\n`)
	})

	socket.on('interfaces:set_standard_digital_out', ({ n, b }) => {
		// make sure to use exactly 'True' or 'False' as bool value.
		const capitalB = String(b).charAt(0).toUpperCase() + String(b).slice(1)
		robots.send(socket.data.robot, `set_standard_digital_out(${n},${capitalB})\n`)
	})
}