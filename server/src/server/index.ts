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