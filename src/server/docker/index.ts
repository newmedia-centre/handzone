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
	_slotMachine: Set<number>

	constructor() {
		// initialize the EventEmitter
		super()

		// create the dockerode instance
		console.log('Connecting to docker...')
		this.docker = new Docker(env.DOCKER.OPTIONS)
		this.containers = new Map()
		this._semaphore = semaphore(env.DOCKER.MAX_VIRTUAL)
		this._slotMachine = new Set([...Array(env.DOCKER.MAX_VIRTUAL).keys()].map(x => x + 1))
		this._semaphore.take(env.DOCKER.MAX_VIRTUAL, () => { })

		// ping docker to check connection
		this.docker.ping((err) => {
			if (err) {
				console.error('Error connecting to docker:', err)
			} else {
				console.log('Connected to docker!')
				this._semaphore.leave(env.DOCKER.MAX_VIRTUAL)

				// close all virtual robots that are still running on start
				console.log('Closing all virtual robots on start...')
				this.closeAllVirtualRobots()
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

		// aquire next available slot from the slot machine
		const slot = [...this._slotMachine.values()][0]
		if (!slot || !this._slotMachine.delete(slot)) {
			throw new Error('No available slots in the slot machine!')
		}

		// create container
		console.log('Creating virtual robot...')
		const container = await this.docker.createContainer({
			Image: 'ghcr.io/newmedia-centre/ursim_cb3:3.15.8',
			HostConfig: {
				PortBindings: {
					'30001/tcp': [{ HostPort: `3${slot}01` }],
					'30002/tcp': [{ HostPort: `3${slot}02` }],
					'30003/tcp': [{ HostPort: `3${slot}03` }],
					'30004/tcp': [{ HostPort: `3${slot}04` }],
					'30011/tcp': [{ HostPort: `3${slot}11` }],
					'30012/tcp': [{ HostPort: `3${slot}12` }],
					'30013/tcp': [{ HostPort: `3${slot}13` }],
					'5900/tcp': [{ HostPort: `59${slot}` }],
					'6080/tcp': [{ HostPort: `608${slot}` }],
				}
			},
			ExposedPorts: {
				'30001/tcp': {},
				'30002/tcp': {},
				'30003/tcp': {},
				'30004/tcp': {},
				'30011/tcp': {},
				'30012/tcp': {},
				'30013/tcp': {},
				'5900/tcp': {},
				'6080/tcp': {},
			},
			Labels: {
				'slot': `${slot}`,
				'handzone': 'virtual'
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
			// release the slot from the slot machine
			const slot = (await container.inspect()).Config.Labels['slot']
			if (!slot) {
				throw new Error('Found container without slot label')
			}
			this._slotMachine.add(parseInt(slot))

			// stop and remove the container
			await container.stop()
			await container.remove()
			this.containers.delete(id)
		}

		// release the semaphore
		this._semaphore.leave(1)
	}

	// close all virtual polyscope instances
	closeAllVirtualRobots = async () => {
		// find all containers with the label 'handzone=virtual'
		const containers = await this.docker.listContainers({ all: true, filters: { label: ['handzone=virtual'] } })

		// remove all found containers
		await Promise.all(containers.map(async container => {
			await this.docker.getContainer(container.Id).stop()
			await this.docker.getContainer(container.Id).remove()
		}))
	}
}

// init tcp server
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const docker: DockerManager = global.docker ?? new DockerManager()
export default docker

// fix global instancing in production // TODO
global.docker = docker
