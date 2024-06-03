// import types
import type { Server } from 'socket.io'
import type {
	ClientToServerEvents,
	ServerToClientEvents,
	InterServerEvents,
	SocketData,
	RobotNamespace
} from '@/server/socket/interface'
import type { RobotManager as RobotManager } from '@/server/robot'
import type { DockerManager } from '@/server/docker'
import type { PrismaClient, User } from '@prisma/client'
import type { Logger } from 'winston'
import type { TOTPController } from 'oslo/otp'

// declare global types
declare global {
	var io: Server<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData> | undefined // eslint-disable-line no-var
	var namespaces: Map<string, RobotNamespace> // eslint-disable-line no-var
	var robots: RobotManager // eslint-disable-line no-var
	var docker: DockerManager // eslint-disable-line no-var
	var prisma: PrismaClient // eslint-disable-line no-var
	var logger: Logger // eslint-disable-line no-var
	var otpPins: Map<string, { user: User, secret: ArrayBuffer }> // eslint-disable-line no-var
	var otpController: TOTPController // eslint-disable-line no-var
	var jwtSecret: ArrayBuffer// eslint-disable-line no-var
}

// declare lucia types for auth
declare module 'lucia' {
	interface Register {
		Lucia: typeof lucia
		DatabaseUserAttributes: User
	}
}