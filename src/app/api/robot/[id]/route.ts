// import dependencies
import { robots } from '@/server/robot'
import { docker } from '@/server/docker'
import { validateRequest } from '@/server/db/auth'

// handle the DELETE request
export async function DELETE(request: Request, { params }: { params: { id: string } }): Promise<Response> {

	// check the user session
	const { session, user } = await validateRequest()
	if (!session || !user.admin) {
		return new Response(null, {
			status: 403
		})
	}

	// get the robot
	const robot = robots.connections.get(params.id)
	if (!robot) {
		return new Response('Robot not found', {
			status: 404
		})
	}

	// close the virtual robot
	if (robot.virtual) {
		docker.closeVirtualRobot(robot.info.name)
	}

	return new Response(null, {
		status: 204
	})
}