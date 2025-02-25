/*
 *The MIT License (MIT)
 * Copyright (c) 2025 NewMedia Centre - Delft University of Technology
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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