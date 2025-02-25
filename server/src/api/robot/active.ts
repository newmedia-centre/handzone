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