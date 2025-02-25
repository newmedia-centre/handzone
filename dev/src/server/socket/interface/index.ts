/*
 * Copyright (c) 2024 NewMedia Centre - Delft University of Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the 'License');
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an 'AS IS' BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
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