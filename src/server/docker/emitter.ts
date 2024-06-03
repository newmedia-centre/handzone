// import types
import type TypedEmitter from 'typed-emitter'

// type all the TCP events
type DockerEvents = {
	/** Emitted when spawning a new docker container would exceed capacity */
	capacity: () => void
}

// export the typed emitters
export type DockerEmitter = TypedEmitter<DockerEvents>