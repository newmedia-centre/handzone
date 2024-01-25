// import dependencies
import { Server } from 'socket.io'
import { initNamespace } from './namespace'

// import types
import type { ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData } from './interface'
import type TCPServer from '../tcp'

// create socket.io server
export const initSocket = (tcp: TCPServer) => {
	// create server instance
	const server = new Server<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData>({
		serveClient: true,
		cors: {
			origin: true,
			methods: ['GET', 'POST'],
			credentials: true
		}
	})

	// forward read and write events
	tcp.on('join', (address, clients) => {
		server.emit('robots', [...clients.keys()])

		// create the namespace if it doesn't exist
		server._nsps.has(`/${address}`) || initNamespace(server.of(`/${address}`), address, tcp)
	})

	tcp.on('leave', (address, clients) => {
		server.emit('robots', [...clients.keys()])

		// delete the namespace if it exists
		server._nsps.has(`/${address}`) && server._nsps.delete(`/${address}`)
	})

	server.on('connection', (socket) => {
		console.log(`Socket ${socket.id} connected`)
		socket.emit('robots', [...tcp.connections.keys()])
	})

	return server
}

export default initSocket