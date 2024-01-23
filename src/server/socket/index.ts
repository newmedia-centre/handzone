// import dependencies
import { Server } from 'socket.io'

// import types
import type { ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData } from './interfaces'

// create socket.io server
export const initSocket = () => {
	// create server instance
	const server = new Server<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData>({
		serveClient: true,
		cors: {
			origin: true,
			methods: ['GET', 'POST'],
			credentials: true
		}
	})

	return server
}