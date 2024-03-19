import type { RealtimeData } from '@/types/Socket/Realtime'

// declare socket.io interfaces
export interface RealtimeServerToClient {
	/**
	 * The raw data from the robot, encoded as a base64 string.
	 */
	'realtime:raw': (data: string) => void

	/** The parsed data from the robot */
	'realtime:data': (data: RealtimeData) => void
}