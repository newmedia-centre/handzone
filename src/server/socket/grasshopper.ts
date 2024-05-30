// import types
import type { Socket } from 'socket.io'
import type { NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData } from './interface'

export const handleGrasshopperEvents = (socket: Socket<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>) => {
	// handle the grasshopper:program event
	socket.on('grasshopper:program', (data) => {
		socket.data.robot.send(data.program + '\n')
	})
}