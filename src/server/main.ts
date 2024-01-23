// import dependencies
import express from 'express'
import ViteExpress from 'vite-express'
import env from './environment'

// create express app
const app = express()

// handle express routing
app.get('/hello', (_, res) => {
	res.send('Hello Vite + React + TypeScript!')
})

// start the server
const server = app.listen(env.PORT, () => {
	console.log(`Server is listening on port ${env.PORT}...`)
	env.ROBOTS?.forEach(x => console.log('Robots: ' + x))
})

// let vite manage the server
ViteExpress.bind(app, server).catch(err => console.error(err))