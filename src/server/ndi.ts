// import dependencies
import { EventEmitter } from 'events'
import ndi from 'grandiose'

// import types
import type { Server } from './socket/interface'
import type TypedEmitter from 'typed-emitter'

/** Manager for NDI connections */
export class NDIManager {
	finder: ndi.GrandioseFinder
	receivers: Map<string, VideoReceiver>

	// constructor
	constructor(ips: string[], io: Server, callback: (self: NDIManager) => void) {
		this.receivers = new Map()
		this.finder = new ndi.GrandioseFinder({
			showLocalSources: true,
			extraIPs: [...ips]
		})

		// wait for 5 seconds to get all the ndi sources
		setTimeout(async () => {
			const sources = this.finder.getCurrentSources()
			console.log('Found NDI sources', sources)

			// create a receiver for each source
			const res = sources.map(async source => {
				const receiver = await ndi.receive({
					source
				})

				// emit the video frames
				const video = new VideoReceiver(receiver)
				video.on('video', frame => {
					io.emit('video', frame.data)
				})

				// add the receiver to the map
				this.receivers.set(source.name, new VideoReceiver(receiver))
			})

			await Promise.all(res)

			// call the callback
			callback.call(this, this)
		}, 5000)
	}
}

/** Listens to a video feed */
class VideoReceiver extends (EventEmitter as new () => ReceiverEmitter) {
	receiver: ndi.Receiver

	constructor(receiver: ndi.Receiver) {
		super()

		this.receiver = receiver
		this.getVideo()
	}

	// recursively get the next video frame
	async getVideo() {
		try {
			//const data = await this.receiver.video(500)
			const any = await this.receiver.data(500)
			console.log(this.receiver.source, any)
			//console.log(data)
			//this.emit('video', data)
			this.getVideo()
		} catch (e) {
			//console.error(e)
			this.getVideo()
		}
	}
}

// type all the NDI events
type ReceiverEvents = {
	/** Emitted when a new video frame is receiver */
	video: (data: ndi.VideoFrame) => void
}

export type ReceiverEmitter = TypedEmitter<ReceiverEvents>