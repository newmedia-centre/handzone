/*
 *The MIT License (MIT)
 * Copyright (c) 2025 NewMedia Centre - Delft University of Technology
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

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
	token: (callback: CallbackFn<string>) => void
}

export interface NamespaceSocketData {
	user: User
	robot: RobotConnection
	color: string
	paused: boolean
	type: 'vr' | 'gh'
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