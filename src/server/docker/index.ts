// import dependencies
import Docker from 'dockerode'
import env from '../environment'
import EventEmitter from 'events'
import semaphore from 'semaphore'

// import types
import type { Semaphore } from 'semaphore'

export class DockerManager extends EventEmitter {
	docker: Docker
	containers: Map<string, Docker.Container>
	semaphore: Semaphore

	constructor() {
		// initialize the EventEmitter
		super()

		// create the dockerode instance
		console.log('Connecting to docker...')
		this.docker = new Docker(env.DOCKER_OPTIONS)
		this.containers = new Map()
		this.semaphore = semaphore(env.MAX_VIRTUAL)

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
		if (!this.semaphore.available(1)) {
			console.log('Virtual robot capacity reached!')
			this.emit('capacity')
		}

		// acquire a semaphore
		await new Promise(resolve => {
			console.log('Waiting for virtual robot capacity...')

			// acquire the semaphore
			this.semaphore.take(1, () => resolve(true))
		})

		// create container
		console.log('Creating virtual robot...')
		const container = await this.docker.createContainer({
			Image: 'universalrobots/ursim_cb3:3.15.8',
			NetworkingConfig: {
				EndpointsConfig: {
					'handzone-network': {},
				}
			}
		})

		// start container
		await container.start()

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
		this.semaphore.leave(1)
	}
}