// import dependencies
import express from 'express'
import ViteExpress from 'vite-express'
import env from './environment'
import TCPServer from './tcp'

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
const tcp = new TCPServer(env.READ_PORT, env.WRITE_PORT, () => {
	console.log(`TCP Server is listening on port ${env.READ_PORT}...`)
})

// let vite manage the server
ViteExpress.bind(app, server).catch(err => console.error(err))