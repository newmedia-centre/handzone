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