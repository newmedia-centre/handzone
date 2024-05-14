// import dependencies
import Express from 'express'
import Next from 'next'
import { createServer } from 'http'
import { io } from './socket'
import { docker } from './docker'
import { robots } from './robot'
import { env } from './environment'

// create the nextjs webserver
const dev = env.NODE_ENV !== 'production'
const next = Next({ dev })
const handle = next.getRequestHandler()

// prepare the nextjs webserver
next.prepare().then(() => {
	try {
		// create an express webserver
		const express = Express()

		// handle all requests through nextjs
		express.all('*', (req, res) => {
			handle(req, res)
		})

		// create the http server
		const server = createServer(express)

		// listen on port 3000
		const instance = server.listen(env.PORT, () => {
			console.log(`Server is running on http://localhost:${env.PORT}`)

			// start the docker manager
			docker.requestVirtualRobot().then(robot => {
				console.log('Virtual Robot:', robot.NetworkSettings.Networks[env.DOCKER_NETWORK]?.IPAddress)
				robots.connectVirtualRobot(robot)
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
		console.error(e)
		process.exit(1)
	}
})