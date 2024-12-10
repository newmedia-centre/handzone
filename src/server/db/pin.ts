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
