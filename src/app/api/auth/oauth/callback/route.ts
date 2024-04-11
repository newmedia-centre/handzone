// import dependencies
import { OAuth2RequestError } from 'oslo/oauth2'
import { cookies } from 'next/headers'
import { oauth, lucia, getUserInfo, decodeIdToken } from '@/server/db/auth'
import { prisma } from '@/server/db'
import { env } from '@/server/environment'

// import types
import type { TokenResponseBody } from 'oslo/oauth2'

// handle the GET request
export async function GET(request: Request): Promise<Response> {
	const url = new URL(request.url)
	const code = url.searchParams.get('code')
	const state = url.searchParams.get('state')
	const storedState = cookies().get('oauth_state')?.value ?? null
	const storedCodeVerifier = cookies().get('oauth_code_verifier')?.value ?? null
	if (!code || !state || !storedState || !storedCodeVerifier || state !== storedState) {
		return new Response(null, {
			status: 400
		})
	}

	try {
		// validate the authorization code and get the tokens
		const tokens = await oauth.validateAuthorizationCode<TokenResponseBody & { id_token: string }>(code, {
			credentials: env.OAUTH_CLIENT_SECRET,
			codeVerifier: storedCodeVerifier,
			authenticateWith: 'request_body'
		})

		// check if the user exists
		const { sub } = decodeIdToken(tokens.id_token)
		const existingUser = await prisma.user.findUnique({
			where: {
				id: sub
			}
		})

		if (existingUser) {
			// create a session
			const session = await lucia.createSession(existingUser.id, {})
			const sessionCookie = lucia.createSessionCookie(session.id)
			cookies().set(sessionCookie.name, sessionCookie.value, sessionCookie.attributes)
		} else {
			// get the user info
			const user = await getUserInfo(tokens.access_token)

			// create the user
			await prisma.user.create({
				data: {
					id: user.id,
					email: user.email,
					name: user.name,
				}
			})

			const session = await lucia.createSession(user.id, {})
			const sessionCookie = lucia.createSessionCookie(session.id)
			cookies().set(sessionCookie.name, sessionCookie.value, sessionCookie.attributes)
		}

		// clear the cookies
		cookies().delete('oauth_state')
		cookies().delete('oauth_code_verifier')

		// redirect to the home page
		return new Response(null, {
			status: 302,
			headers: {
				Location: '/'
			}
		})
	} catch (e) {
		// the specific error message depends on the provider
		if (e instanceof OAuth2RequestError) {
			// invalid code
			return new Response(null, {
				status: 400
			})
		}
		return new Response(null, {
			status: 500
		})
	}
}