// import types
import type { Server } from 'socket.io'
import type { Namespace } from '@/server/socket/namespace'
import type {
	ClientToServerEvents,
	ServerToClientEvents,
	InterServerEvents,
	SocketData
} from '@/server/socket/interface'
import type { RobotManager as RobotManager } from '@/server/robot'
import type { DockerManager } from '@/server/docker'
import type { PrismaClient } from '@prisma/client'

// declare global types
declare global {
	var io: Server<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData> | undefined // eslint-disable-line no-var
	var namespaces: Map<string, Namespace> // eslint-disable-line no-var
	var robots: RobotManager // eslint-disable-line no-var
	var docker: DockerManager // eslint-disable-line no-var
	var prisma: PrismaClient // eslint-disable-line no-var
}
