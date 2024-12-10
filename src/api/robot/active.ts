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