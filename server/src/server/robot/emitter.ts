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
import type TypedEmitter from 'typed-emitter'
import type { RobotConnection } from './connection'
import type { RealtimeDataOut } from '@/types/Socket/Realtime'

// type all the TCP events
type ManagerEvents = {
	/** Emitted when a new connection is established or a connection is updated */
	join: (connection: RobotConnection, clients: Map<string, RobotConnection>) => void
	/**  */
	leave: (connection: RobotConnection, clients: Map<string, RobotConnection>) => void
}

// type all the robot events
type RobotEvents = {
	/** Emitted when the robot sends a message */
	message: (message: string) => void
	/** A raw realtime buffer */
	'realtime:raw': (buffer: Buffer) => void
	/** A parsed realtime buffer */
	'realtime:parsed': (data: RealtimeDataOut) => void
	/** A request response */
	response: (response: Buffer) => void
}

// type all the vnc events
type VNCEvents = {
	/** Emitted when a vnc buffer is available */
	data: (data: Buffer) => void
}


// type all the video events
type VideoEvents = {
	/** Emitted when a video frame is available */
	frame: (data: Buffer) => void
}

// export the typed emitters
export type ManagerEmitter = TypedEmitter<ManagerEvents>
export type RobotEmitter = TypedEmitter<RobotEvents>
export type VNCEmitter = TypedEmitter<VNCEvents>
export type VideoEmitter = TypedEmitter<VideoEvents>
