// import dependencies
import { TOTPController } from 'oslo/otp'
import { TimeSpan } from 'oslo'
import { HMAC } from 'oslo/crypto'

// import types
import type { User } from '@prisma/client'

// init the TOTP controller and pin map
const pins = new Map<string, { user: User, secret: ArrayBuffer }>()
const controller = new TOTPController({ digits: 6, period: new TimeSpan(15, 'm') })

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

	// set a timeout to delete the pin after it expires
	setTimeout(() => { pins.delete(otp) }, 15 * 60 * 1000)

	return otp
}

// validate the pin and return the user
export const validatePin = async (otp: string) => {
	// get the user and secret
	const data = pins.get(otp)
	if (!data) {
		throw new Error('Invalid pin')
	}

	// verify the pin
	const valid = await controller.verify(otp, data.secret)
	if (!valid) {
		throw new Error('Invalid pin')
	}

	// return the user
	pins.delete(otp)
	return data.user
}