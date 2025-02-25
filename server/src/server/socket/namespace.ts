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
import { handleInterfacesEvents } from './interfaces'
import { handleInternalsEvents } from './internals'
import { handleRealtimeEvents } from './realtime'
import { handleMotionEvents } from './motion'
import { handleGrasshopperEvents } from './grasshopper'
import { handleUnityEvents } from './unity'
import { validateAccessToken, generateAccessToken } from '@/server/db/jwt'
import { docker } from '@/server/docker'

// import types
import type { Namespace } from 'socket.io'
import type { NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData } from './interface'
import type { PlayerData } from '@/types/Socket/Unity'
import type { RobotConnection } from '../robot/connection'
import type { Logger } from 'winston'

/** Initialize a new namespace by handling all the required events */
export const initNamespace = (namespace: Namespace<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>, robot: RobotConnection, logger: Logger) => {

	// set up the namespace middleware
	namespace.use((socket, next) => {
		// get the token
		const token = socket.handshake.auth.token as string

		// check if the pin is valid
		validateAccessToken(token).then(({ user }) => {
			// attach the user to the socket
			socket.data.user = user
			socket.data.robot = robot
			socket.data.type = socket.handshake.auth.type as typeof socket.data.type
			socket.data.color = '#' + (Math.random() * 0xFFFFFF << 0).toString(16)

			return next()
		}).catch(e => {
			logger.warn('Could not authenticate user', { error: e })
			return next(new Error('User not authenticated'))
		})
	})

	// create a map to store player data
	const players = new Map<string, PlayerData>()

	// handle the connection to the namespace
	namespace.on('connection', socket => {
		logger.http(`Socket ${socket.id} connected`, { user: socket.data.user })

		// handle socket disconnection
		socket.on('disconnect', () => {
			// do nothing
		})

		// handle all the incoming events
		handleMotionEvents(socket)
		handleGrasshopperEvents(socket, logger)
		handleRealtimeEvents(socket)
		handleInterfacesEvents(socket)
		handleInternalsEvents(socket)
		handleUnityEvents(socket, players)

		// forward video events
		robot.video?.forEach(video => {
			video.on('frame', (data) => {
				socket.emit('video', video.camera.name, data.toString('base64'))
			})
		})

		// forward events between sockets

		// handle the message event
		socket.on('message', (message) => {
			socket.broadcast.emit('message', message)
		})

		// send a new token
		socket.on('token', async (callback) => {
			generateAccessToken(socket.data.user, socket.data.robot.info)
				.then(token => callback(true, token))
				.catch(e => callback(false, e.message))
		})

		// remove player data on disconnect
		socket.on('disconnect', () => {
			players.delete(socket.id)

			// close the virtual robot connection if no users are connected
			if (robot.virtual && namespace.sockets.size <= 0) {
				docker.closeVirtualRobot(robot.info.name)
			}
		})
	})

	// emit the positions data
	setInterval(() => {
		namespace.emit('unity:players', { players: Array.from(players.values()) })
	}, 100)

	logger.info('Namespace initialized')
	return namespace
}