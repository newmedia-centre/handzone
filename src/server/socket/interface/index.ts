// import types
import type { Socket as BareSocket, Server as BareServer, Namespace } from 'socket.io'
import type { MotionClientToServer } from './motion'
import type { GrasshopperClientToServer, GrasshopperServerToClient } from './grasshopper'
import type { UnityClientToServer, UnityServerToClient } from './unity'
import type { InterfacesClientToServer } from './interfaces'
import type { InternalsClientToServer } from './internals'
import type { RealtimeServerToClient } from './realtime'
import type { RobotConnection, RobotInfo } from '@/server/robot/connection'
import type { SessionsOut, JoinSessionOut, SessionType } from '@/types/Socket/Index'
import type { User } from '@prisma/client'

export type CallbackFn<T> = {
	(success: true, payload: T): void
	(success: false, message: string): void
}

// declare socket.io interfaces
export interface ServerToClientEvents {
	message: (message: string) => void
	sessions: (payload: SessionsOut) => void
	join: (payload: JoinSessionOut) => void
	queue: (position: number) => void
	afk: () => void

}

export interface ClientToServerEvents {
	message: (message: string) => void
	real: (callback: CallbackFn<JoinSessionOut>) => void
	virtual: (type: SessionType, callback: CallbackFn<JoinSessionOut>) => void
	join: (address: string, callback: CallbackFn<JoinSessionOut>) => void
	namespace: (callback: CallbackFn<JoinSessionOut>) => void
	achievement: () => void
	afk: () => void
}

export interface InterServerEvents {
	ping: () => void
}

export interface SocketData {
	user: User
	namespace?: RobotInfo
}

export interface NamespaceServerToClientEvents extends GrasshopperServerToClient, UnityServerToClient, RealtimeServerToClient {
	message: (message: string) => void
	video: (camera: string, frame: string) => void
}

export interface NamespaceClientToServerEvents extends MotionClientToServer, GrasshopperClientToServer, UnityClientToServer, InterfacesClientToServer, InternalsClientToServer {
	message: (message: string) => void
}

export interface NamespaceSocketData {
	user: User
	robot: RobotConnection
	color: string
	paused: boolean
	achievements: {
		id: string
		data: JSON
	}[]
}

// create the socket type
export type Socket = BareSocket<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>
export type Server = BareServer<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>

// export namespace type
export type RobotNamespace = {
	robot: RobotConnection
	nsp: Namespace<NamespaceClientToServerEvents, NamespaceServerToClientEvents, InterServerEvents, NamespaceSocketData>
}