// import dependencies
import { cookies } from 'next/headers'
import { lucia, validateRequest } from '@/server/db/auth-next'

// handle the GET request
export async function GET(): Promise<Response> {
	const { session } = await validateRequest()
	if (!session) {
		return new Response(null, {
			status: 302,
			headers: {
				Location: '/about'
			}
		})
	}

	await lucia.invalidateSession(session.id)

	const sessionCookie = lucia.createBlankSessionCookie()
	cookies().set(sessionCookie.name, sessionCookie.value, sessionCookie.attributes)
	return new Response(null, {
		status: 302,
		headers: {
			Location: '/about'
		}
	})
}