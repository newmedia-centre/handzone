// import types
import type { Socket as BareSocket, Server as BareServer } from 'socket.io'
import type { MotionClientToServer } from './motion'
import type { GrasshopperClientToServer, GrasshopperServerToClient } from './grasshopper'
import type { UnityClientToServer, UnityServerToClient } from './unity'
import type { InterfacesClientToServer } from './interfaces'
import type { InternalsClientToServer } from './internals'
import type { RealtimeServerToClient } from './realtime'
import type { RobotConnection } from '@/server/robot'
import type { RobotsOut, JoinSessionOut } from '@/types/Socket/Index'
import type { User } from '@prisma/client'

// declare socket.io interfaces
export interface ServerToClientEvents {
	message: (message: string) => void
	robots: (payload: RobotsOut) => void
	join: (payload: JoinSessionOut) => void
	queue: (position: number) => void
	afk: () => void

}

export interface ClientToServerEvents {
	message: (message: string) => void
	virtual: (callback: (success: boolean, payload?: JoinSessionOut) => void) => void
	join: (address: string, callback: (success: boolean, payload?: JoinSessionOut) => void) => void
	achievement: () => void
	afk: () => void
}

export interface InterServerEvents {
	ping: () => void
}

export interface SocketData {
	user: User
}

export interface NamespaceServerToClientEvents extends GrasshopperServerToClient, UnityServerToClient, RealtimeServerToClient {
	message: (message: string) => void
	video: (camera: string, frame: string) => void
	vnc: (data: string) => void
	token: (token: string) => void
	'vnc:init': (data: string) => void
}

export interface NamespaceClientToServerEvents extends MotionClientToServer, GrasshopperClientToServer, UnityClientToServer, InterfacesClientToServer, InternalsClientToServer {
	message: (message: string) => void
	vnc: (data: string) => void
	'vnc:pixelformat': (data: string) => void
}

export interface NamespaceSocketData {
	user: User
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