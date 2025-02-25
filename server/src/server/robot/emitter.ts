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
