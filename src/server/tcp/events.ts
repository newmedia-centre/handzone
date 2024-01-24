// import types
import type TypedEmitter from 'typed-emitter'
import type { Socket } from 'net'
import type { ReadConnection } from '.'

// type all the TCP events
type TCPEvents = {
	/** Emitted when a new read connection is established */
	read: (sockets: Map<string, ReadConnection>) => void
	/** Emitted when a new write connection is established */
	write: (sockets: Map<string, Socket>) => void
}

// type all the robot events
type RobotEvents = {
	/** Emitted when the robot sends a message */
	message: (message: string) => void
}

// export the typed emitters
export type TCPEmitter = TypedEmitter<TCPEvents>
export type RobotEmitter = TypedEmitter<RobotEvents>