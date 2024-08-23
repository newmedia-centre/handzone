// import dependencies
import { validateApi } from '@/server/db/auth'
import { generatePin } from '@/server/db/pin'

// import types
import type { Request, Response } from 'express'

// create the pin route
export const pin = async (req: Request, res: Response) => {
	// check the user session
	const { session, user } = await validateApi(req, res)
	if (!session) {
		res.status(403)
		return res.send()
	}

	// generate and send the pin
	const pin = await generatePin(user)
	res.status(200)
	res.send(pin)
}