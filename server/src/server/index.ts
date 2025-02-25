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
import Express from 'express'
import Next from 'next'
import { createServer } from 'http'
import { io } from './socket'
import { docker } from './docker'
import { robots } from './robot'
import { env } from './environment'
import { webLogger as logger } from './logger'
import { api } from '@/api'

// create the nextjs webserver
const dev = env.NODE_ENV !== 'production'
const next = Next({ dev })
const handle = next.getRequestHandler()

// create an express webserver
const express = Express()
express.use('/api', api)

// prepare the nextjs webserver
next.prepare().then(() => {
	try {

		// handle all other requests through nextjs
		express.all('*', (req, res) => {
			handle(req, res)
		})

		// create the http server
		const server = createServer(express)

		// listen on port 3000
		const instance = server.listen(env.PORT, () => {
			logger.info(`Server is running on http://localhost:${env.PORT}`)

			// start the docker manager
			docker.requestVirtualRobot().then(robot => {
				logger.debug('Virtual Robot:', env.DOCKER.OPTIONS.host, robot.Config.Labels['slot'])
				robots.connectVirtualRobot(robot, 'sandbox')
			})

			// attach the socket.io server
			io.attach(instance, {
				serveClient: true,
				maxHttpBufferSize: 1e8,
				cors: {
					origin: true,
					methods: ['GET', 'POST'],
					credentials: true
				}
			})
		})
	} catch (e) {
		logger.error('Error starting server', { error: e })
		process.exit(1)
	}
})