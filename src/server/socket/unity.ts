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
import type { NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData } from './interface'
import type { PlayerData } from '@/types/Socket/Unity'

export const handleUnityEvents = (socket: Socket<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>, players: Map<string, PlayerData>) => {

	// handle the unity:message event
	socket.on('unity:message', ({ message }) => {

		// send the message to all clients
		socket.broadcast.emit('unity:message', ({ message }))
	})

	// handle the unity:position event
	socket.on('unity:player', ({ hmd, left, right, cursor }) => {
		players.set(socket.id, {
			id: socket.id,
			hmd,
			left,
			right,
			cursor,
			name: socket.data.user.name ?? '',
			color: socket.data.color
		})
	})

	// handle the unity:pendant event
	socket.on('unity:pendant', () => {
		if (socket.data.robot.paused) return
		socket.nsp.emit('unity:pendant', { owner: socket.id })
	})

	// handle the unity:run event
	socket.on('unity:run', ({ run }) => {
		console.log('run', run)
		socket.data.robot.send(run ? 'resume program\n' : 'pause program\n')
	})
}