// import dependencies
import { Server } from 'socket.io'
import { initNamespace } from './namespace'
import env from '../environment'

// import types
import type { ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData } from './interfaces'
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

	// map the real and virtual robots into a single array
	const targets = [
		...(env.ROBOTS ? env.ROBOTS.map(x => ({ ...x, type: 'real' as const })) : []),
		...(env.VIRTUAL_ROBOTS ? env.VIRTUAL_ROBOTS.map(x => ({ ...x, type: 'virtual' as const })) : [])
	]

	// create a namespace for each robot and virtual robot
	targets.forEach(target => {

		// create and initialize the namespace
		const namespace = server.of(`/${target.slug}`)
		initNamespace(namespace, target, tcp)

	})

	return server
}

export default initSocket