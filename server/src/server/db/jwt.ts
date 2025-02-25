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

// import dependencies
import { HMAC } from 'oslo/crypto'
import { createJWT, validateJWT } from 'oslo/jwt'
import { TimeSpan } from 'oslo'
import { databaseLogger } from '../logger'

// import types
import type { User } from '@prisma/client'
import type { RobotInfo } from '@/server/robot/connection'

// create logger
export const logger = databaseLogger.child({ entity: 'jwt', label: 'DB:JWT' })

// generate secret for signing JWTs
const generateSecret = async () => { global.jwtSecret = await new HMAC('SHA-256').generateKey() }
if (!global.jwtSecret) { generateSecret() }

// generate an access token for the user to join a robot session
export const generateAccessToken = async (user: User, robot: RobotInfo) => await createJWT('HS256', global.jwtSecret, {
	user,
	robot
}, {
	expiresIn: new TimeSpan(1, 'm'),
	includeIssuedTimestamp: true
})

// validate the access token and return the user and robot
export const validateAccessToken = async (token: string) => {
	const jwt = await validateJWT('HS256', global.jwtSecret, token)
	const payload = jwt.payload as { user: User, robot: RobotInfo }

	if (!payload.user || !payload.robot) {
		logger.info('Log in attempt without valid jwt')
		throw new Error('Invalid access token')
	}

	return payload
}