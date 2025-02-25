/*
 * Copyright (c) 2024 NewMedia Centre - Delft University of Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the 'License');
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an 'AS IS' BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

// import dependencies
import { EventEmitter } from 'events'
import { spawn } from 'child_process'
import { Buffer } from 'buffer'
import type env from '../environment'

// import types
import type { ChildProcess } from 'child_process'
import type { VideoEmitter } from './emitter'
import type { Logger } from 'winston'

export type CameraInfo = typeof env['ROBOTS'][number]['camera'][number]

export class VideoConnection extends (EventEmitter as new () => VideoEmitter) {
	process?: ChildProcess
	camera: CameraInfo
	logger: Logger

	constructor(camera: CameraInfo, logger: Logger) {
		// initialize the EventEmitter
		super()

		this.camera = camera
		this.logger = logger

		// initialize the ffmpeg process
		logger.info('Starting ffmpeg process...', camera)
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
			logger.info('ffmpeg process started', camera)
		})

		// log any errors from the ffmpeg process
		process.on('error', err => this.logger.error('ffmpeg process error', { error: err }))

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