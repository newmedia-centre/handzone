// import dependencies
import { EventEmitter } from 'events'
import { Buffer } from 'buffer'
import { parseRealtimeData } from '@/server/socket/realtime'
import { VideoConnection } from './video'

// import types
import type { RobotEmitter } from './events'
import type { SessionType } from '@/types/Socket/Index'
import type { VNCProxy } from './proxy'
import type { Socket } from 'net'
import type env from '../environment'

export type RobotInfo = typeof env['ROBOTS'][number]

/** Listens for data from a robot over a TCP socket */
export class RobotConnection extends (EventEmitter as new () => RobotEmitter) {
	/** The TCP socket for reading data */
	virtual: SessionType | null
	socket: Socket
	vnc?: VNCProxy
	interval?: NodeJS.Timeout
	realtimeBuffer?: Buffer
	video?: Set<VideoConnection>
	info: RobotInfo

	constructor(robot: Socket, info: RobotInfo, virtual: SessionType | null) {
		// initialize the EventEmitter
		super()

		// initialize the class variables
		this.virtual = virtual
		this.socket = robot
		this.video = new Set()
		this.info = info

		// initialize the video connection if the robot has a camera
		info.camera.forEach(camera => {
			this.video?.add(new VideoConnection(camera))
		})

		// start the interval at 25hz which should be enough for most applications
		this.interval = setInterval(() => this.handleRealtimeData(), 40)

		// handle incoming messages
		this.socket.on('data', (data) => {
			// check if the data is realtime data
			const header = this.getRealtimeHeader(data)

			// check if the buffer length is the length of the realtime buffer
			if (header) {
				this.realtimeBuffer = data
			} else {
				// parse the data

				// emit the message
				this.emit('response', data)
			}
		})

	}

	clear() {
		clearTimeout(this.interval)
	}

	async handleRealtimeData() {
		// get the latest buffer
		const data = this.realtimeBuffer
		if (!data) return

		// emit the raw realtime data
		this.emit('realtime:raw', data)

		// parse the realtime data
		const parsed = await parseRealtimeData(data)
		this.emit('realtime:parsed', parsed)
	}

	getRealtimeHeader(data: Buffer) {
		// verify the size of the package, realtime data has a fixed size of 1220 bytes, if so, return true
		return data.length % 1220 === 0
	}

	/** Sends an instruction to the robot */
	async send(instruction: string) {
		// send the instruction as a utf-8 buffer
		this.socket.write(Buffer.from(instruction, 'utf-8'))
	}
}