// import dependencies
import { EventEmitter } from 'events'
import { Socket, createServer } from 'net'
import { spawn } from 'child_process'
import { parseRealtimeData } from '@/server/socket/realtime'
import { Buffer } from 'buffer'
import { VNCConnection } from './vnc'
import semaphore from 'semaphore'
import env from '../environment'

// import types
import type { Semaphore } from 'semaphore'
import type { ChildProcess } from 'child_process'
import type { RobotEmitter, ManagerEmitter, VideoEmitter } from './events'
import type { ContainerInspectInfo } from 'dockerode'

type RobotInfo = typeof env['ROBOTS'][number]
type CameraInfo = typeof env['ROBOTS'][number]['camera'][number]

/** The TCP Server for communicating with the robots */
export class RobotManager extends (EventEmitter as new () => ManagerEmitter) {
	/** The TCP Sockets for sending messages through the TCP Server */
	connections: Map<string, RobotConnection>
	_semaphore: Semaphore

	constructor() {
		// initialize the EventEmitter
		super()

		// initialize the class variables
		this.connections = new Map()
		this._semaphore = semaphore(1)

		// try to connect to the robots
		env.ROBOTS.forEach((robot) => {
			this._tryCreateRobotConnection(robot)
		})
	}

	/** Sends an instruction to the robot */
	async send(robot: RobotConnection, instruction: string) {
		// send the instruction as a utf-8 buffer
		robot.socket.write(Buffer.from(instruction, 'utf-8'))
	}

	/** sends an instruction with a callback */
	async sendCallback(robot: RobotConnection, instruction: string) {
		// acquire a semaphore
		await new Promise(resolve => {
			this._semaphore.take(1, () => resolve(true))
		})

		const promise = new Promise<Buffer>((resolve, reject) => {
			// set timeout to 5 seconds
			const timeout = setTimeout(() => {
				server.close()
				server.on('close', () => { this._semaphore.leave(1) })

				reject('timeout')
			}, 5000)

			// create a tcp server receive values from the robot and listen on port 4000
			const server = createServer(socket => {

				socket.once('data', data => {
					clearTimeout(timeout)
					server.close()
					server.on('close', () => { this._semaphore.leave(1) })
					resolve(data)
				})

				socket.on('error', () => { })
			})
			server.maxConnections = 1
			server.listen(4000)
		})

		// send the instruction as a utf-8 buffer
		robot.socket.write(Buffer.from(instruction, 'utf-8'))

		return promise
	}

	/** Tries to connect to an endpoint */
	async connectVirtualRobot(container: ContainerInspectInfo) {
		this._tryCreateRobotConnection({
			name: container.Id,
			address: this._parseAddress(container.NetworkSettings.Networks[env.DOCKER_NETWORK]?.IPAddress),
			port: 30003,
			vnc: 5900,
			camera: []
		})
	}

	// private methods

	/** Parses the given RemoteAddress into an IPv4 Address */
	_parseAddress(address?: string) {
		if (!address) throw new Error('Address is undefined')

		if (address.includes(':')) {
			return address.replace(/^.*:/, '')
		}

		return address
	}

	/** Starts the TCP Client */
	_tryCreateRobotConnection(robot: RobotInfo) {
		// create the TCP client
		const socket = new Socket()
		socket.setTimeout(5000)
		console.info(`[ROBOT:${robot.address}] Connecting...`)
		socket.connect(robot.port, robot.address)

		// initialize the vnc connection if the robot has a vnc port
		if (robot.vnc) {
			new VNCConnection(robot)
		}

		// retry until a connection is established
		socket.on('error', (error: NodeJS.ErrnoException) => {
			if (error.code === 'ECONNREFUSED') {
				return setTimeout(() => {
					console.info(`[ROBOT:${robot.address}] Retrying...`)
					socket.connect(robot.port, robot.address)
				}, socket.timeout || 1000)
			}
			console.error(error)
		})

		// add clients when connected
		socket.on('connect', () => {
			const connection = new RobotConnection(socket, robot)
			this.connections.set(robot.address, connection)
			this.emit('join', connection, this.connections)
			console.info(`[ROBOT:${robot.address}] Connected`)
		})

		// remove from clients when closed
		socket.on('close', () => {
			if (this.connections.has(robot.address)) {
				const connection = this.connections.get(robot.address)
				this.emit('leave', connection!, this.connections)
				connection?.clear()
				this.connections.delete(robot.address)
				console.info(`[ROBOT:${robot.address}] Disconnected`)
			}
		})
	}

}

/** Listens for data from a robot over a TCP socket */
export class RobotConnection extends (EventEmitter as new () => RobotEmitter) {
	/** The TCP socket for reading data */
	socket: Socket
	vnc?: VNCConnection
	interval?: NodeJS.Timeout
	realtimeBuffer?: Buffer
	video?: Set<VideoConnection>
	info: RobotInfo

	constructor(robot: Socket, info: RobotInfo) {
		// initialize the EventEmitter
		super()

		// initialize the class variables
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
}

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

// init tcp server
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const robots: RobotManager = global.robots ?? new RobotManager()
export default robots

// fix global instancing in production // TODO
global.robots = robots
