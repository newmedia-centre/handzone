// import payload types from shared schema
import type {
	UnityMessageIn,
	UnityMessageOut,
	UnityPendantIn,
	UnityPendantOut,
	UnityPlayerIn,
	UnityPlayersOut
} from '@/types/Socket/Unity'

export interface UnityClientToServer {
	'unity:message': (payload: UnityMessageIn) => void
	'unity:player': (payload: UnityPlayerIn) => void
	'unity:pendant': (payload: UnityPendantIn) => void
}

export interface UnityServerToClient {
	'unity:message': (payload: UnityMessageOut) => void
	'unity:players': (payload: UnityPlayersOut) => void
	'unity:pendant': (payload: UnityPendantOut) => void
}