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