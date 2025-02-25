/*
 *The MIT License (MIT)
 * Copyright (c) 2025 NewMedia Centre - Delft University of Technology
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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