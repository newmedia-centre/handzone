// import dependencies
import { env } from '@/server/environment'

// import types
import type { Socket } from 'socket.io'
import type { NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData } from './interface'

export const handleInternalsEvents = (socket: Socket<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>) => {
	socket.on('internals:get_inverse_kin', async ({ x, qnear, maxPositionError, tcp_offset }, callback) => {
		const instruction = `def get_value():\nsocket_open("${env.HOSTNAME}", ${env.TCP_PORT}, "socket_value")\nvalue=get_inverse_kin(p[${x}]${qnear ? `, qnear=${qnear}` : ''}${maxPositionError ? `, maxPositionError=${maxPositionError}` : ''}${tcp_offset ? `, tcp=${tcp_offset}` : ''})\nsocket_send_string(to_str(value), "socket_value")\nend\n`
		console.log('instruction:', instruction)
		try {
			const res = await socket.data.robot.sendCallback(instruction)
			const ik = JSON.parse(res.toString('utf8'))
			callback(true, { ik })
		} catch (e) {
			console.log(e)
			callback(false, 'Error getting inverse kinematics')
		}
	})
}