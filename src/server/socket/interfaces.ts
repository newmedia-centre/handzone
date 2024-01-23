// import types
import type { Socket as BareSocket } from 'socket.io'

// declare socket.io interfaces
export interface ServerToClientEvents {

}

export interface ClientToServerEvents {

}

export interface InterServerEvents {
	ping: () => void
}

export interface SocketData {
	id: string
}

// create the socket type
export type Socket = BareSocket<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData>