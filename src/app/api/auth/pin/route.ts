// import dependencies
import { validateRequest } from '@/server/db/auth'
import { generatePin } from '@/server/db/pin'

// handle the GET request
export async function GET(): Promise<Response> {
	// check the user session
	const { session, user } = await validateRequest()
	if (!session) {
		return new Response(null, {
			status: 403
		})
	}

	// generate and send the pin
	const pin = await generatePin(user)
	return new Response(pin, {
		status: 200
	})
}