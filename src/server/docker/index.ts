// import dependencies
import Docker from 'dockerode'
import env from '../environment'
import EventEmitter from 'events'
import { Sema } from 'async-sema'
import { dockerLogger as logger } from '../logger'

// import types
import type { DockerEmitter } from './emitter'

export class DockerManager extends (EventEmitter as new () => DockerEmitter) {
	docker: Docker
	containers: Map<string, Docker.Container>
	_semaphore: Sema
	_slotMachine: Set<number>

	constructor() {
		// initialize the EventEmitter
		super()

		// create the dockerode instance
		logger.info('Connecting to docker...')
		this.docker = new Docker(env.DOCKER.OPTIONS)
		this.containers = new Map()
		this._semaphore = new Sema(env.DOCKER.MAX_VIRTUAL)
		this._slotMachine = new Set([...Array(env.DOCKER.MAX_VIRTUAL).keys()].map(x => x + 1))

		// drain the semaphore until docker is available
		this._semaphore.drain()

		// ping docker to check connection
		this.docker.ping(async (err) => {
			if (err) {
				logger.error('Error connecting to docker', { error: err })
			} else {
				logger.info('Connected to docker')

				// close all virtual robots that are still running on start
				logger.info('Closing all virtual robots on start...')
				await this.closeAllVirtualRobots()

				logger.info('Docker available')
				for (let i = 0; i < env.DOCKER.MAX_VIRTUAL; i++) {
					this._semaphore.release()
				}
			}
		})
	}

	// request a new virtual polyscope instance, which will be spawned once it is available
	requestVirtualRobot = async () => {
		logger.info('Requesting virtual robot...')

		// emit a capacity event if the semaphore is full
		if (this._semaphore['free'].length <= 0) {
			logger.info('Virtual robot capacity reached')
			this.emit('capacity')
		}

		// acquire a semaphore
		logger.info('Waiting for virtual robot capacity...')
		await this._semaphore.acquire()
		logger.info('Virtual robot capacity acquired', this._semaphore['free'].length)

		// aquire next available slot from the slot machine
		const slot = [...this._slotMachine.values()][0]
		if (!slot || !this._slotMachine.delete(slot)) {
			logger.error('No available slots in the slot machine')
			throw new Error('No available slots in the slot machine')
		}

		// create container
		logger.info('Creating virtual robot...')
		const container = await this.docker.createContainer({
			Image: 'ghcr.io/newmedia-centre/ursim_cb3:3.15.8',
			HostConfig: {
				RestartPolicy: { Name: 'always' },
				PortBindings: {
					'30001/tcp': [{ HostPort: `3${slot.toString().padStart(2, '0')}01` }],
					'30002/tcp': [{ HostPort: `3${slot.toString().padStart(2, '0')}02` }],
					'30003/tcp': [{ HostPort: `3${slot.toString().padStart(2, '0')}03` }],
					'30004/tcp': [{ HostPort: `3${slot.toString().padStart(2, '0')}04` }],
					'30011/tcp': [{ HostPort: `3${slot.toString().padStart(2, '0')}11` }],
					'30012/tcp': [{ HostPort: `3${slot.toString().padStart(2, '0')}12` }],
					'30013/tcp': [{ HostPort: `3${slot.toString().padStart(2, '0')}13` }],
					'5900/tcp': [{ HostPort: `59${slot.toString().padStart(2, '0')}` }],
					'6080/tcp': [{ HostPort: `608${slot.toString().padStart(2, '0')}` }],
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
				'slot': `${slot.toString().padStart(2, '0')}`,
				'handzone': 'virtual'
			},
		})

		// start container
		await container.start()

		// return the container info
		const name = await this._getName(container)
		this.containers.set(name, container)
		return await container.inspect()
	}

	// close a virtual polyscope instance
	closeVirtualRobot = async (name: string) => {
		// remove the container from the map
		const container = this.containers.get(name)
		if (container) {
			// release the slot from the slot machine
			const slot = (await container.inspect()).Config.Labels['slot']
			if (!slot) {
				logger.error('Found container without slot label!')
				throw new Error('Found container without slot label')
			}
			this._slotMachine.add(parseInt(slot))

			// stop and remove the container
			await container.stop()
			await container.remove()
			this.containers.delete(name)
		}

		// release the semaphore
		this._semaphore.release()
	}

	// close all virtual polyscope instances
	closeAllVirtualRobots = async () => {
		// find all containers with the label 'handzone=virtual'
		const containers = await this.docker.listContainers({ all: true, filters: { label: ['handzone=virtual'] } })

		// remove all found containers
		await Promise.all(containers.map(async container => {
			const name = await this._getName(this.docker.getContainer(container.Id))
			await this.docker.getContainer(container.Id).stop().catch(err => logger.warn('Error closing virtual robot', { error: err }))
			await this.docker.getContainer(container.Id).remove().catch(err => logger.error('Error closing virtual robot', { error: err }))
			this.containers.delete(name)
		}))
	}

	// get the current capacity of the semaphore
	getCapacity = () => {
		return this._semaphore['free'].length
	}

	_getName = async (container: Docker.Container) => {
		const info = await container.inspect()
		return info.Name.split('/')[1]!
	}
}

// init tcp server
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const docker: DockerManager = global.docker ?? new DockerManager()
export default docker

global.docker = docker
