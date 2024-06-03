// import dependencies
import { TOTPController } from 'oslo/otp'
import { TimeSpan } from 'oslo'
import { HMAC } from 'oslo/crypto'
import { env } from '@/server/environment'
import { databaseLogger } from '../logger'

// import types
import type { User } from '@prisma/client'

// create logger
export const logger = databaseLogger.child({ entity: 'pin', label: 'DB:PIN' })

// init the TOTP controller and pin map
const pins = global.otpPins || new Map<string, { user: User, secret: ArrayBuffer }>()
const controller = global.otpController || new TOTPController({ digits: env.NODE_ENV === 'development' ? 2 : 4, period: new TimeSpan(15, 'm') })

// generate a pin for the user
export const generatePin = async (user: User) => {
	// delete any existing pins for the user
	for (const [otp, data] of pins) {
		if (data.user.id === user.id) {
			pins.delete(otp)
		}
	}

	// generate a new pin
	const secret = await new HMAC('SHA-256').generateKey()
	const otp = await controller.generate(secret)
	pins.set(otp, { user, secret })
	logger.info('Generated pin for user', { user: user.id, otp })

	// set a timeout to delete the pin after it expires
	setTimeout(() => { pins.delete(otp) }, 15 * 60 * 1000)

	return otp
}

// validate the pin and return the user
export const validatePin = async (otp: string) => {
	// get the user and secret
	const data = pins.get(otp)
	if (!data) {
		logger.info('Log in attempt without valid pin (not found)', { otp, pins: pins.keys() })
		throw new Error('Invalid pin (not found)')
	}

	// verify the pin
	const valid = await controller.verify(otp, data.secret)
	if (!valid) {
		logger.info('Log in attempt without valid pin (invalid)')
		throw new Error('Invalid pin (invalid)')
	}

	// return the user
	pins.delete(otp)
	return data.user
}

global.otpPins = pins
global.otpController = controller
