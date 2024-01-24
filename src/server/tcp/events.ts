// import types
import type TypedEmitter from 'typed-emitter'

// type all the TCP events
type TCPEvents = {
	/** Emitted when a new connection is established or a connection is updated */
	join: (address: string, clients: Map<string, { read: boolean, write: boolean }>) => void
	/**  */
	leave: (address: string, clients: Map<string, { read: boolean, write: boolean }>) => void
}

// type all the robot events
type RobotEvents = {
	/** Emitted when the robot sends a message */
	message: (message: string) => void
}

// export the typed emitters
export type TCPEmitter = TypedEmitter<TCPEvents>
export type RobotEmitter = TypedEmitter<RobotEvents>