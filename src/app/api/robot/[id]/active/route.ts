// import dependencies
import { z } from 'zod'
import { robots } from '@/server/robot'
import { validateRequest } from '@/server/db/auth'

// handle the POST request
export async function POST(request: Request, { params }: { params: { id: string } }): Promise<Response> {

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

	// create the request body parser
	const Data = z.object({
		active: z.boolean(),
	})

	// parse the request body
	const body = await request.json()
	const parsed = Data.safeParse(body)

	// validate the request body
	if (!parsed.success) {
		return new Response(parsed.error.message, {
			status: 400
		})
	}

	robot.active = parsed.data.active
	return new Response(null, {
		status: 204
	})
}