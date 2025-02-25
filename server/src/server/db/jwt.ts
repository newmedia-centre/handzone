/*
 * Copyright (c) 2024 NewMedia Centre - Delft University of Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the 'License');
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an 'AS IS' BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
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