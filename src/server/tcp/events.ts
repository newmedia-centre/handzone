// import types
import type TypedEmitter from 'typed-emitter'
import type { Socket } from 'net'

// type all the TCP events
type TCPEvents = {
	/** Emitted when a new connection is established or a connection is updated */
	join: (address: string, clients: Map<string, Socket>) => void
	/**  */
	leave: (address: string, clients: Map<string, Socket>) => void
}

// type all the robot events
type RobotEvents = {
	/** Emitted when the robot sends a message */
	message: (message: string) => void
}

// export the typed emitters
export type TCPEmitter = TypedEmitter<TCPEvents>
export type RobotEmitter = TypedEmitter<RobotEvents>