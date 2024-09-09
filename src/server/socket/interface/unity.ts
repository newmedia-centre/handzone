// import payload types from shared schema
import type {
	UnityMessageIn,
	UnityMessageOut,
	UnityPendantOut,
	UnityPlayerIn,
	UnityPlayersOut,
	UnityRunIn
} from '@/types/Socket/Unity'

export interface UnityClientToServer {
	'unity:message': (payload: UnityMessageIn) => void
	'unity:player': (payload: UnityPlayerIn) => void
	'unity:pendant': () => void
	'unity:run': (payload: UnityRunIn) => void
}

export interface UnityServerToClient {
	'unity:message': (payload: UnityMessageOut) => void
	'unity:players': (payload: UnityPlayersOut) => void
	'unity:pendant': (payload: UnityPendantOut) => void
}