// import dependencies
import Express from 'express'
import Next from 'next'
import { createServer } from 'http'
import { initSocket } from './socket';

// create the nextjs webserver
const dev = process.env.NODE_ENV !== "production";
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
		const instance = server.listen(3000, () => {
			console.log('Server is running on http://localhost:3000')

			// attach the socket.io server
			const socket = initSocket()
			socket.attach(instance, {
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