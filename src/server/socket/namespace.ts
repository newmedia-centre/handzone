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
import { handleInterfacesEvents } from './interfaces'
import { handleInternalsEvents } from './internals'
import { handleRealtimeEvents } from './realtime'
import { handleMotionEvents } from './motion'
import { handleGrasshopperEvents } from './grasshopper'
import { handleUnityEvents } from './unity'
import { validateAccessToken } from '@/server/db/jwt'
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
		handleGrasshopperEvents(socket)
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