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
import { Router, json } from 'express'
import { validateApi } from '@/server/db/auth'

// import handlers
import { deleteRobot } from './manage'
import { active } from './active'
import { pause } from './pause'

// create the robot router
export const robot = Router()

// set up middleware
robot.use(json())

// only allow admin to operate the robot
robot.use(async (req, res, next) => {
	// check the user session
	const { session, user } = await validateApi(req, res)
	if (!session || !user?.admin) {
		return res.status(403).send()
	}

	return next()
})

// add the route handlers
robot.delete('/:id', deleteRobot)
robot.post('/:id/active', active)
robot.post('/:id/pause', pause)