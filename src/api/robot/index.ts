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