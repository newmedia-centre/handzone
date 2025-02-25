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
import { z } from 'zod'
import { validateApi } from '@/server/db/auth'
import { generatePin, validatePin } from '@/server/db/pin'

// import types
import type { Request, Response } from 'express'

// create the pin POST route
export const postPin = async (req: Request, res: Response) => {
	// create the request body parser
	const Data = z.object({
		signature: z.string(),
	})

	// parse the request body
	const body = req.body
	const parsed = Data.safeParse(body)

	// validate the request body
	if (!parsed.success) {
		return res.status(400).send(parsed.error.message)
	}

	// generate and send the pin
	try {
		const pin = await generatePin(parsed.data.signature)
		return res.status(200).send(pin)
	} catch (error) {
		return res.status(400).send((error as Error).message)
	}
}

// create the pin PUT route
export const putPin = async (req: Request, res: Response) => {
	// check the user session
	const { session, user } = await validateApi(req, res)
	if (!session) {
		return res.status(403).send()
	}

	// create the request body parser
	const Data = z.object({
		pin: z.string(),
	})

	// parse the request body
	const body = req.body
	const parsed = Data.safeParse(body)

	// validate the request body
	if (!parsed.success) {
		console.log(parsed.error)
		return res.status(400).send(parsed.error.message)
	}

	// claim the pin
	await validatePin(parsed.data.pin, user)

	res.status(200)
	res.send()
}