// import types
import type { Socket as BareSocket, Server as BareServer } from 'socket.io'
import type { MotionClientToServer } from './motion'
import type { GrasshopperClientToServer, GrasshopperServerToClient } from './grasshopper'
import type { UnityClientToServer, UnityServerToClient } from './unity'
import type { InterfacesClientToServer } from './interfaces'
import type { InternalsClientToServer } from './internals'
import type { RealtimeServerToClient } from './realtime'
import type { RobotConnection } from '@/server/robot'

// declare socket.io interfaces
export interface ServerToClientEvents {
	message: (message: string) => void
	robots: (robots: string[]) => void
	queue: (position: number) => void
	afk: () => void
}

export interface ClientToServerEvents {
	message: (message: string) => void
	virtual: () => void
	achievement: () => void
}

export interface InterServerEvents {
	ping: () => void
}

export interface SocketData {
	dbid: string
}

export interface NamespaceServerToClientEvents extends GrasshopperServerToClient, UnityServerToClient, RealtimeServerToClient {
	message: (message: string) => void
	video: (camera: string, frame: string) => void
	vnc: (data: string) => void
}

export interface NamespaceClientToServerEvents extends MotionClientToServer, GrasshopperClientToServer, UnityClientToServer, InterfacesClientToServer, InternalsClientToServer {
	message: (message: string) => void
	vnc: (data: string) => void
	'vnc:pixelformat': (data: string) => void
}

export interface NamespaceSocketData {
	dbid: string
	robot: RobotConnection
	name: string
	color: string
	achievements: {
		id: string
		data: JSON
	}[]
}

// create the socket type
export type Socket = BareSocket<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>
export type Server = BareServer<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>