// import types
import type { Socket as BareSocket } from 'socket.io'
import type { RTDEClientToServer, RTDEServerToClient } from './rtde/interface'
import type { MotionClientToServer } from './motion/interface'
import type { InterfacesClientToServer } from './interfaces/interface'
import type { RealtimeServerToClient } from './realtime/interface'

/** Represents a 6D vector consisting of three force components and three torque components. */
export type Vector6D = {
	/** X-axis */
	x: number
	/** Y-axis */
	y: number
	/** Z-axis */
	z: number
	/** U-axis */
	u: number
	/** V-axis */
	v: number
	/** W-axis */
	w: number
}

/** Represents a 3D vector consisting of three components */
export type Vector3D = {
	/** X-axis */
	x: number
	/** Y-axis */
	y: number
	/** Z-axis */
	z: number
}

// declare socket.io interfaces
export interface ServerToClientEvents extends RTDEServerToClient, RealtimeServerToClient {
	message: (message: string) => void
	simulation: () => void
	robots: (robots: string[]) => void
}

export interface ClientToServerEvents extends RTDEClientToServer, MotionClientToServer, InterfacesClientToServer {
	message: (message: string) => void
	simulation: () => void
	achievement: () => void
}

export interface InterServerEvents {
	ping: () => void
}

export interface SocketData {
	id: string
	robot: string
}

// create the socket type
export type Socket = BareSocket<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData>