// import dependencies
import express from 'express'
import ViteExpress from 'vite-express'
import env from './environment'
import TCPServer from './tcp'
import initSocket from './socket'
import { NDIManager } from './ndi'
import { strFromU8, gzipSync } from 'fflate'

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

// start and attach the socket.io server
const io = initSocket(tcp)
io.attach(server)

// start the ndi video manager
new NDIManager(['172.19.14.27', '172.19.14.158', '172.19.14.230', '172.19.14.152'], (ndi) => {
	ndi.receivers.forEach((receiver, name) => {
		receiver.on('video', frame => {
			if (name === 'NewTek PTZ Camera (Chan 1)') {
				const namespace = io._nsps.get('/172.19.14.251')!
				namespace.emit('video', strFromU8(gzipSync(frame.data)))
				console.log('Got video frame:', name, frame.data.length)
			}
		})
	})
})


// let vite manage the server
ViteExpress.bind(app, server).catch(err => console.error(err))