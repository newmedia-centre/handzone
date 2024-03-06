// import dependencies
import { EventEmitter } from 'events'
import { Socket } from 'net'
import { spawn } from 'child_process'
import { parseRealtimeData } from '@/server/socket/realtime'
import { Buffer } from 'buffer'
import env from '../environment'

// import types
import type { ChildProcess } from 'child_process'
import type { RobotEmitter, TCPEmitter, VideoEmitter } from './events'
import type { ContainerInspectInfo } from 'dockerode'

type robotInfo = typeof env['ROBOTS'][number]

/** The TCP Server for communicating with the robots */
export class TCPServer extends (EventEmitter as new () => TCPEmitter) {
	/** The TCP Sockets for sending messages through the TCP Server */
	connections: Map<string, RobotConnection>

	constructor() {
		// initialize the EventEmitter
		super()

		// initialize the class variables
		this.connections = new Map()

		// try to connect to the robots
		env.ROBOTS.forEach((robot) => {
			this._tryCreateRobotConnection(robot)
		})
	}

	/** Sends an instruction to the rover */
	async send(address: string, instruction: string) {
		// get the socket
		const robot = this.connections.get(address)
		if (!robot) throw new Error(`No write connection found for address ${address}`)

		// send the instruction as a utf-8 buffer
		robot.socket.write(Buffer.from(instruction, 'utf-8'))
	}

	/** Tries to connect to an endpoint */
	async connectVirtualRobot(container: ContainerInspectInfo) {
		this._tryCreateRobotConnection({
			name: container.Id,
			address: container.NetworkSettings.Networks[env.DOCKER_NETWORK].IPAddress,
			port: 30003
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
	_tryCreateRobotConnection(robot: robotInfo) {
		// create the TCP client
		const socket = new Socket()
		socket.setTimeout(5000)
		console.info(`[ROBOT:${robot.address}] Connecting...`)
		socket.connect(robot.port, robot.address)

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
			this.emit('join', robot.address, this.connections)
			console.info(`[ROBOT:${robot.address}] Connected`)
		})

		// remove from clients when closed
		socket.on('close', () => {
			if (this.connections.has(robot.address)) {
				this.connections.get(robot.address)?.clear()
				this.connections.delete(robot.address)
				this.emit('join', robot.address, this.connections)
				console.info(`[ROBOT:${robot.address}] Disconnected`)
			}
		})
	}

}

/** Listens for data from a robot over a TCP socket */
export class RobotConnection extends (EventEmitter as new () => RobotEmitter) {
	/** The TCP socket for reading data */
	socket: Socket
	interval?: NodeJS.Timeout
	realtimeBuffer?: Buffer
	video?: VideoConnection

	constructor(socket: Socket, robot: robotInfo) {
		// initialize the EventEmitter
		super()

		// initialize the class variables
		this.socket = socket

		// initialize the video connection if the robot has a camera
		if (robot.camera) {
			this.video = new VideoConnection(robot.camera)
		}

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
				const message = data.toString('utf-8')
				//console.log('received message:', data.length)

				// emit the message
				this.emit('message', message)
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
		// verify the size of the package, if it doesn't match the header, return undefined for not being RTDE data
		const size = data.readUInt32BE(0)
		if (size % data.length !== 0) return undefined

		return {
			size
		}
	}

	getRTDEHeader(data: Buffer) {
		// verify the size of the package, if it doesn't match the header, return undefined for not being RTDE data
		const size = data.readUInt16BE(0)
		if (size !== data.length) return undefined

		const type = data.readUInt8(2)
		switch (type) {
			case 86: return {
				size,
				type: 'RTDE_REQUEST_PROTOCOL_VERSION'
			}
			case 118: return {
				size,
				type: 'RTDE_GET_URCONTROL_VERSION'
			}
			case 77: return {
				size,
				type: 'RTDE_TEXT_MESSAGE'
			}
			case 85: return {
				size,
				type: 'RTDE_DATA_PACKAGE'
			}
			case 79: return {
				size,
				type: 'RTDE_CONTROL_PACKAGE_SETUP_OUTPUTS'
			}
			case 73: return {
				size,
				type: 'RTDE_CONTROL_PACKAGE_SETUP_INPUTS'
			}
			case 83: return {
				size,
				type: 'RTDE_CONTROL_PACKAGE_START'
			}
			case 80: return {
				size,
				type: 'RTDE_CONTROL_PACKAGE_PAUSE'
			}
			default: return undefined
		}
	}
}

export class VideoConnection extends (EventEmitter as new () => VideoEmitter) {
	/** The TCP socket for reading data */
	process?: ChildProcess

	constructor(url: string) {
		// initialize the EventEmitter
		super()

		// initialize the ffmpeg process
		console.log('Starting ffmpeg process...')
		const process = spawn('ffmpeg', [
			'-rtsp_transport', 'tcp',
			'-i', url,
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
export const tcp = global.tcp ?? new TCPServer()
export default tcp

// fix global instancing in production // TODO
global.tcp = tcp
