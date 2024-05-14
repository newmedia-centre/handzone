// import dependencies
import Docker from 'dockerode'
import env from '../environment'
import EventEmitter from 'events'
import semaphore from 'semaphore'

// import types
import type { Semaphore } from 'semaphore'
import type { DockerEmitter } from './events'

export class DockerManager extends (EventEmitter as new () => DockerEmitter) {
	docker: Docker
	containers: Map<string, Docker.Container>
	_semaphore: Semaphore

	constructor() {
		// initialize the EventEmitter
		super()

		// create the dockerode instance
		console.log('Connecting to docker...')
		this.docker = new Docker(env.DOCKER.OPTIONS)
		this.containers = new Map()
		this._semaphore = semaphore(env.DOCKER.MAX_VIRTUAL)

		// ping docker to check connection
		this.docker.ping((err) => {
			if (err) {
				console.error('Error connecting to docker:', err)
			} else {
				console.log('Connected to docker!')
			}
		})
	}

	// request a new virtual polyscope instance, which will be spawned once it is available
	requestVirtualRobot = async () => {
		console.log('Requesting virtual robot...')

		// emit a capacity event if the semaphore is full
		if (!this._semaphore.available(1)) {
			console.log('Virtual robot capacity reached!')
			this.emit('capacity')
		}

		// acquire a semaphore
		await new Promise(resolve => {
			console.log('Waiting for virtual robot capacity...')

			// acquire the semaphore
			this._semaphore.take(1, () => resolve(true))
		})

		// create container
		console.log('Creating virtual robot...')
		const container = await this.docker.createContainer({
			Image: 'ghcr.io/newmedia-centre/ursim_cb3:3.15.8',
			NetworkingConfig: {
				EndpointsConfig: {
					[env.DOCKER_NETWORK]: {},
				}
			}
		})

		// start container
		await container.start()

		// wait some time for the container to start
		await new Promise(resolve => setTimeout(resolve, 20000))

		// return the container info
		this.containers.set(container.id, container)
		return await container.inspect()
	}

	// close a virtual polyscope instance
	closeVirtualRobot = async (id: string) => {
		// remove the container from the map
		const container = this.containers.get(id)
		if (container) {
			await container.stop()
			await container.remove()
			this.containers.delete(id)
		}

		// release the semaphore
		this._semaphore.leave(1)
	}
}

// init tcp server
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const docker: DockerManager = global.docker ?? new DockerManager()
export default docker

// fix global instancing in production // TODO
global.docker = docker
