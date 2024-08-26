// import dependencies
import { EventEmitter } from 'events'
import { createServer } from 'net'
import { Buffer } from 'buffer'
import { parseRealtimeData } from '@/server/socket/realtime'
import { VideoConnection } from './video'
import { robots } from '.'

// import types
import type { RobotEmitter } from './emitter'
import type { SessionType } from '@/types/Socket/Index'
import type { VNCProxy } from './vnc'
import type { Socket } from 'net'
import type { Logger } from 'winston'
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
	logger: Logger
	active: boolean = false
	paused: boolean = false

	constructor(robot: Socket, info: RobotInfo, virtual: SessionType | null, logger: Logger) {
		// initialize the EventEmitter
		super()

		// initialize the class variables
		this.virtual = virtual
		this.socket = robot
		this.video = new Set()
		this.info = info
		this.logger = logger

		// initialize the video connection if the robot has a camera
		info.camera.forEach(camera => {
			this.video?.add(new VideoConnection(camera, logger))
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
		this.logger.http(`Sending instruction: ${instruction}`, { instruction })

		// send the instruction as a utf-8 buffer
		this.socket.write(Buffer.from(instruction, 'utf-8'))
	}


	/** sends an instruction with a callback */
	async sendCallback(instruction: string) {
		this.logger.http(`Sending instruction with callback: ${instruction}`, { instruction })

		// acquire a semaphore
		await robots._semaphore.acquire()

		const promise = new Promise<Buffer>((resolve, reject) => {
			// set timeout to 5 seconds
			let timeout = setTimeout(() => {
				server.close()
				server.on('close', () => { robots._semaphore.release() })

				reject(new Error('Timeout'))
			}, 5000)

			// create a tcp server receive values from the robot and listen on port 4000
			const server = createServer(socket => {

				// update timeout
				clearTimeout(timeout)
				timeout = setTimeout(() => {
					server.close()
					server.on('close', () => { robots._semaphore.release() })

					reject(new Error('Timeout'))
				}, 50)

				socket.once('data', data => {
					clearTimeout(timeout)
					server.close()
					server.on('close', () => { robots._semaphore.release() })

					this.logger.http(`Received callback for instruction: ${instruction}`, { instruction, data })
					resolve(data)
				})

				socket.on('error', () => {
					this.logger.warn(`Received error for instruction: ${instruction}`, { instruction })
				})
			})
			server.maxConnections = 1
			server.listen(4000)
		})

		// send the instruction as a utf-8 buffer
		this.socket.write(Buffer.from(instruction, 'utf-8'))

		return promise
	}
}