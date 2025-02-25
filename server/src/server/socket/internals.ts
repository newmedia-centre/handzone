/*
 * Copyright (c) 2024 NewMedia Centre - Delft University of Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the 'License');
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an 'AS IS' BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// import dependencies
import { env } from '@/server/environment'

// import types
import type { Socket } from 'socket.io'
import type { NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData } from './interface'

export const handleInternalsEvents = (socket: Socket<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>) => {
	socket.on('internals:get_inverse_kin', async ({ x, qnear, maxPositionError, tcp_offset }, callback) => {
		const instruction = `def get_value():\nsocket_open("${env.HOSTNAME}", ${env.TCP_PORT}, "socket_value")\nvalue=get_inverse_kin(p[${x}]${qnear ? `, qnear=${qnear}` : ''}${maxPositionError ? `, maxPositionError=${maxPositionError}` : ''}${tcp_offset ? `, tcp=${tcp_offset}` : ''})\nsocket_send_string(to_str(value), "socket_value")\nend\n`
		try {
			const res = await socket.data.robot.sendCallback(instruction)
			const ik = JSON.parse(res.toString('utf8'))
			callback(true, { ik })
		} catch (e) {
			callback(false, 'Error getting inverse kinematics: ' + (e as Error).message)
		}
	})
}