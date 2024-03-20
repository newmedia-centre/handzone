// import payload types from shared schema
import type {
	RealtimeRawOut,
	RealtimeDataOut
} from '@/types/Socket/Realtime'

// declare socket.io interfaces
export interface RealtimeServerToClient {
	/**
	 * The raw data from the robot, encoded as a base64 string.
	 */
	'realtime:raw': (payload: RealtimeRawOut) => void

	/** The parsed data from the robot */
	'realtime:data': (payload: RealtimeDataOut) => void
}