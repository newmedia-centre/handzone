// import dependencies
import { z } from 'zod'
import { robots } from '@/server/robot'
import { namespaces } from '@/server/socket'
import { webLogger as logger } from '@/server/logger'

// import types
import type { Request, Response } from 'express'

// create the active route
export const active = async (req: Request, res: Response) => {
	// check if an id was provided
	if (!req.params.id) {
		return res.status(400).send()
	}

	// get the robot
	const robot = robots.connections.get(req.params.id)
	if (!robot) {
		logger.http('Robot not found', { robot: req.params.id })
		return res.status(404).send('Robot not found')
	}

	// create the request body parser
	const Data = z.object({
		active: z.boolean(),
	})

	// parse the request body
	const body = req.body
	const parsed = Data.safeParse(body)

	// validate the request body
	if (!parsed.success) {
		return res.status(400).send(parsed.error.message)
	}

	// kick all users when active is false
	if (!parsed.data.active) {
		namespaces.get(req.params.id)?.nsp.disconnectSockets()
	}

	robot.active = parsed.data.active
	return res.status(204).send()
}