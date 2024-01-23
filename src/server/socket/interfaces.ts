// import types
import type { Socket as BareSocket } from 'socket.io'
import type { RTDEClientToServer, RTDEServerToClient } from './rtde/interfaces'
import type { MotionClientToServer } from './motion/interfaces'

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
export interface ServerToClientEvents extends RTDEServerToClient {
	simulation: () => void
}

export interface ClientToServerEvents extends RTDEClientToServer, MotionClientToServer {
	achievement: () => void
	simulation: () => void
}

export interface InterServerEvents {
	ping: () => void
}

export interface SocketData {
	id: string
	target: {
		name: string
		slug: string
		address: string
		type: 'real' | 'virtual'
	}
}

// create the socket type
export type Socket = BareSocket<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData>