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

// import types
import type { Socket } from 'socket.io'
import type { Logger } from 'winston'
import type { NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData } from './interface'

export const handleGrasshopperEvents = (socket: Socket<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>, logger: Logger) => {
	// handle the grasshopper:program event
	socket.on('grasshopper:program', (data) => {
		logger.info('Received program from grasshopper', { data: data.program })
		socket.data.robot.send(data.program + '\n')
	})

	// handle the grasshopper:simulate event
	socket.on('grasshopper:run', (data) => {
		data.run ? socket.data.robot.send('resume program\n') : socket.data.robot.send('pause program\n')
	})

	// handle the grasshopper:simulate event
	socket.on('grasshopper:meshes', (data) => {
		socket.broadcast.emit('grasshopper:meshes', data)
	})
}