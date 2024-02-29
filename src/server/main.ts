// import dependencies
import express from 'express'
import ViteExpress from 'vite-express'
import env from './environment'
import TCPServer from './tcp'
import initSocket from './socket'
import { DockerManager } from './docker'

// create express app
const app = express()

// handle express routing
app.get('/hello', (_, res) => {
	res.send('Hello Vite + React + TypeScript!')
})

// start the server
const server = app.listen(env.WEB_PORT, () => {
	console.log(`Web Server is listening on port ${env.WEB_PORT}...`)
})

// start the tcp server
const tcp = new TCPServer()

// start the docker manager
const docker = new DockerManager()
docker.requestVirtualRobot().then(robot => {
	console.log('Virtual Robot:', robot.NetworkSettings.IPAddress)
})

// start and attach the socket.io server
const io = initSocket(tcp)
io.attach(server)

// let vite manage the server
ViteExpress.bind(app, server).catch(err => console.error(err))

// handle process termination
let closing = false
const close = async () => {
	// check if already closing
	if (closing) return
	closing = true

	// shut down the virtual robots
	console.log('Shutting down virtual robots...')
	const vrobots = Array.from(docker.containers.values()).map(async container => {
		await docker.closeVirtualRobot(container.id)
		console.log('Closed virtual robot:', container.id)
	})

	// close the robot connections
	console.log('Shutting down robots...')
	tcp.connections.forEach((connection, address) => {
		// kill the ffmpeg process
		connection.video?.process?.kill()

		// close the socket connection
		connection.socket.destroy()

		console.log('Closed robot:', address)
	})

	// close the express server
	const express = new Promise(resolve => {
		server.close(() => {
			console.log('Server closed!')
			resolve(true)
		})
	})

	// await all promises
	await Promise.all([...vrobots, express])
	process.exit()
}

process.on('SIGINT', close)
process.on('SIGTERM', close)
process.on('SIGHUP', close)