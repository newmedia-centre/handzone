// import dependencies
import { z } from 'zod'
import { robots } from '@/server/robot'
import { namespaces } from '@/server/socket'
import { validateRequest } from '@/server/db/auth-next'

// handle the POST request
export async function POST(request: Request, { params }: { params: { id: string } }): Promise<Response> {

	// check the user session
	const { session, user } = await validateRequest()
	if (!session || !user.admin) {
		return new Response(null, {
			status: 403
		})
	}

	// get the robot and the robot session
	const robot = robots.connections.get(params.id)
	const namespace = namespaces.get(params.id)
	if (!robot || !namespace) {
		return new Response('Robot or robot session not found', {
			status: 404
		})
	}

	// create the request body parser
	const Data = z.object({
		paused: z.boolean(),
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

	robot.paused = parsed.data.paused
	namespace.nsp.emit('unity:pendant', { owner: 'paused' })
	return new Response(null, {
		status: 204
	})
}