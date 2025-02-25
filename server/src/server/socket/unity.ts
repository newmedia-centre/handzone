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