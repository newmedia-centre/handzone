// import dependencies
import { HMAC } from 'oslo/crypto'
import { createJWT, validateJWT } from 'oslo/jwt'
import { TimeSpan } from 'oslo'
import type { env } from '../environment'

// import types
import type { User } from '@prisma/client'

// generate secret for signing JWTs
let secret: ArrayBuffer
const generateSecret = async () => { secret = await new HMAC('SHA-256').generateKey() }
generateSecret()

type RobotInfo = typeof env['ROBOTS'][number]

// generate an access token for the user to join a robot session
export const generateAccessToken = async (user: User, robot: RobotInfo) => await createJWT('HS256', secret, {
	user,
	robot
}, {
	expiresIn: new TimeSpan(1, 'm'),
	includeIssuedTimestamp: true
})

// validate the access token and return the user and robot
export const validateAccessToken = async (token: string) => {
	const jwt = await validateJWT('HS256', secret, token)
	const payload = jwt.payload as { user: User, robot: RobotInfo }

	if (!payload.user || !payload.robot) {
		throw new Error('Invalid access token')
	}

	return payload
}