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
import { TOTPController } from 'oslo/otp'
import { TimeSpan } from 'oslo'
import { base64 } from 'oslo/encoding'
import { env } from '@/server/environment'
import { databaseLogger } from '../logger'

// import types
import type { User } from '@prisma/client'

// create logger
export const logger = databaseLogger.child({ entity: 'pin', label: 'DB:PIN' })

// init the TOTP controller and pin map
const pins = global.otpPins || new Map<string, User | null>()
const controller = global.otpController || new TOTPController({ digits: env.NODE_ENV === 'development' ? 2 : 4, period: new TimeSpan(15, 'm') })

// generate a pin for the user
export const generatePin = async (signature: string) => {

	// generate a new pin
	const otp = await controller.generate(base64.decode(signature))
	pins.set(otp, null)
	logger.info('Generated pin', { otp })

	// set a timeout to delete the pin after it expires
	setTimeout(() => { pins.delete(otp) }, 15 * 60 * 1000)

	return otp
}

// claims the pin for the given user
export const validatePin = async (otp: string, user: User) => {
	// get the user and secret
	const data = pins.get(otp)
	if (data === undefined) {
		logger.info('Invalid pin (not found)', { otp, pins: pins.keys() })
		throw new Error('Invalid pin (not found)')
	}

	// check if the pin is already claimed
	if (data !== null) {
		logger.info('Pin already validated', { otp, user: data })
		throw new Error('Pin already validated')
	}

	// claim the pin
	pins.set(otp, user)
	logger.info('Validated pin', { otp, user })
}

// verify the pin and return the user
export const verifyPin = async (otp: string, signature: string) => {
	// get the user and secret
	const data = pins.get(otp)
	if (data === undefined) {
		logger.info('Verification attempt without valid pin (not found)', { otp, pins: pins.keys() })
		throw new Error('Invalid pin (not found)')
	}

	// verify the pin
	const valid = await controller.verify(otp, base64.decode(signature))
	if (!valid) {
		logger.info('Verification attempt without valid pin (invalid)')
		throw new Error('Invalid pin (invalid)')
	}

	// return the user and the access token
	return data
}

global.otpPins = pins
global.otpController = controller
