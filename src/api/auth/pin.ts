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
	console.log(body)
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