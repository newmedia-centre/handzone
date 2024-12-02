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
import { robots } from '@/server/robot'
import { docker } from '@/server/docker'

// import types
import type { Request, Response } from 'express'

// create the delete route
export const deleteRobot = async (req: Request, res: Response) => {
	// check if an id was provided
	if (!req.params.id) {
		return res.status(400).send()
	}

	// get the robot
	const robot = robots.connections.get(req.params.id)
	if (!robot) {
		return res.status(404).send('Robot not found')
	}

	// close the virtual robot
	if (robot.virtual) {
		docker.closeVirtualRobot(robot.info.name)
	}

	return res.status(204).send()
}