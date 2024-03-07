// import dependencies
import type { Namespace } from 'socket.io'
import { Server } from 'socket.io'
import { initNamespace } from './namespace'
import { robots } from '../robot'

// import types
import type {
	ClientToServerEvents,
	ServerToClientEvents,
	InterServerEvents,
	SocketData,
	NamespaceClientToServerEvents,
	NamespaceServerToClientEvents,
	NamespaceSocketData
} from './interface'

// create socket.io server
export const init = () => {
	// create server instance
	const server = new Server<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData>({
		serveClient: true,
		maxHttpBufferSize: 1e8,
		cors: {
			origin: true,
			methods: ['GET', 'POST'],
			credentials: true
		}
	})

	// forward read and write events
	robots.on('join', (address, clients) => {
		server.emit('robots', [...clients.keys()])

		// create the namespace if it doesn't exist
		server._nsps.has(`/${address}`) || initNamespace((server.of(`/${address}`) as unknown) as Namespace<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>, address, clients.get(address)!)
	})

	robots.on('leave', (address, clients) => {
		server.emit('robots', [...clients.keys()])

		// delete the namespace if it exists
		server._nsps.has(`/${address}`) && server._nsps.delete(`/${address}`)
	})

	server.on('connection', (socket) => {
		console.log(`Socket ${socket.handshake.address}, ${socket.id} connected`)
		socket.emit('robots', [...robots.connections.keys()])
	})

	return server
}

// init socket.io server
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const io: Server<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData> = global.io ?? init()
export default io

// fix global instancing in production // TODO
global.io = io
