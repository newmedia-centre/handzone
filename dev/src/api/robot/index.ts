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