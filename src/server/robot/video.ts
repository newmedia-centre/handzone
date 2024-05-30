// import dependencies
import { EventEmitter } from 'events'
import { spawn } from 'child_process'
import { Buffer } from 'buffer'
import type env from '../environment'

// import types
import type { ChildProcess } from 'child_process'
import type { VideoEmitter } from './events'

export type CameraInfo = typeof env['ROBOTS'][number]['camera'][number]

export class VideoConnection extends (EventEmitter as new () => VideoEmitter) {
	process?: ChildProcess
	camera: CameraInfo

	constructor(camera: CameraInfo) {
		// initialize the EventEmitter
		super()

		this.camera = camera

		// initialize the ffmpeg process
		console.log('Starting ffmpeg process...')
		const process = spawn('ffmpeg', [
			'-rtsp_transport', 'tcp',
			'-i', camera.address,
			'-f', 'image2',
			'-update', '1',
			'-loglevel', 'quiet',
			'pipe:1'], {
			stdio: ['inherit', 'pipe', 'inherit']
		})

		let buffer = Buffer.alloc(0)

		// log once the process has received data
		process.stdout.once('data', () => {
			console.log('ffmpeg process started')
		})

		// read image frames from ffmpeg stdout and send to connected clients
		process.stdout.on('data', (data: Buffer) => {
			if (data.length === 8192) {
				buffer = Buffer.concat([buffer, data])
			} else {
				buffer = Buffer.concat([buffer, data])
				this.emit('frame', buffer)
				buffer = Buffer.alloc(0)
			}
		})

		this.process = process
	}
}