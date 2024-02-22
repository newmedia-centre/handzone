// import types
import type TypedEmitter from 'typed-emitter'
import type { RobotConnection } from '.'
import type { RealtimeData } from '@/types/Socket/Realtime/RealtimeData'

// type all the TCP events
type TCPEvents = {
	/** Emitted when a new connection is established or a connection is updated */
	join: (address: string, clients: Map<string, RobotConnection>) => void
	/**  */
	leave: (address: string, clients: Map<string, RobotConnection>) => void
}

// type all the robot events
type RobotEvents = {
	/** Emitted when the robot sends a message */
	message: (message: string) => void
	/** A raw RTDE buffer */
	rtde: (buffer: Buffer) => void
	/** A raw realtime buffer */
	'realtime:raw': (buffer: Buffer) => void
	/** A parsed realtime buffer */
	'realtime:parsed': (data: RealtimeData) => void
}

// type all the video events
type VideoEvents = {
	/** Emitted when a video frame is available */
	frame: (data: Buffer) => void
}

// export the typed emitters
export type TCPEmitter = TypedEmitter<TCPEvents>
export type RobotEmitter = TypedEmitter<RobotEvents>
export type VideoEmitter = TypedEmitter<VideoEvents>
