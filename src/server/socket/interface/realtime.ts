// import payload types from shared schema
import type {
	RealtimeRawOut,
	RealtimeDataOut
} from '@/types/Socket/Realtime'

// declare socket.io interfaces
export interface RealtimeServerToClient {
	'realtime:raw': (payload: RealtimeRawOut) => void
	'realtime:data': (payload: RealtimeDataOut) => void
}