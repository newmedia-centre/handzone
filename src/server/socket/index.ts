// import dependencies
import { Server } from 'socket.io'

export const initSocket = () => {
	const server = new Server()

	return server
}