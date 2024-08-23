// import dependencies
import { validateRequest } from '@/server/db/auth-next'
import { generatePin, validatePin } from '@/server/db/pin'

// handle the POST request
export async function POST(request: Request): Promise<Response> {
	// get the signature
	const { signature } = await request.json()

	// generate and send the pin
	try {
		const pin = await generatePin(signature)
		return new Response(pin, {
			status: 200
		})
	} catch (error) {
		return new Response((error as Error).message, {
			status: 400
		})
	}
}

// handle the PUT request
export async function PUT(request: Request): Promise<Response> {
	try {
		// check the user session
		const { session, user } = await validateRequest()
		if (!session) {
			return new Response(null, {
				status: 403
			})
		}

		// claim the pin
		const { pin } = await request.json()
		await validatePin(pin, user)

		return new Response(null, {
			status: 200
		})
	} catch (error) {
		return new Response((error as Error).message, {
			status: 400
		})
	}
}