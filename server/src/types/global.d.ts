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
	var otpPins: Map<string, User | null> // eslint-disable-line no-var
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