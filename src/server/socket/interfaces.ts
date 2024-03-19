// import dependencies
import { robots } from '@/server/robot'
import { env } from '@/server/environment'

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

	socket.on('interfaces:get_inverse_kin', async ({ x, qnear, maxPositionError, tcp_offset }, callback) => {
		const instruction = `def get_value():\nsocket_open("${env.HOSTNAME}", ${env.TCP_PORT}, "socket_value")\nvalue=get_inverse_kin(p[${x}]${qnear ? `, qnear=${qnear}` : ''}${maxPositionError ? `, maxPositionError=${maxPositionError}` : ''}${tcp_offset ? `, tcp=${tcp_offset}` : ''})\nsocket_send_string(to_str(value), "socket_value")\nend\n`
		console.log('instruction:', instruction)
		try {
			const res = await robots.sendCallback(socket.data.robot, instruction)
			console.log('res:', res.toString('utf8'))
			callback(true, { ik: [] })
		} catch (e) {
			console.log(e)
			callback(false)
		}
	})
}