// import dependencies
import { OAuth2RequestError } from 'oslo/oauth2'
import { oauth, lucia, getUserInfo, decodeIdToken } from '@/server/db/auth'
import { prisma } from '@/server/db'
import { env } from '@/server/environment'

// import types
import type { Request, Response } from 'express'
import type { TokenResponseBody } from 'oslo/oauth2'

// create the oauth route
export const oauthCallback = async (req: Request, res: Response) => {
	const url = new URL(req.url)

	const code = url.searchParams.get('code')
	const state = url.searchParams.get('state')
	const storedState = req.cookies['oauth_state']?.value ?? null
	const storedCodeVerifier = req.cookies['oauth_code_verifier']?.value ?? null
	if (!code || !state || !storedState || !storedCodeVerifier || state !== storedState) {
		return res.status(400).send()
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
			res.cookie(sessionCookie.name, sessionCookie.serialize())
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
			res.cookie(sessionCookie.name, sessionCookie.serialize())
		}

		// clear the cookies
		res.clearCookie('oauth_state')
		res.clearCookie('oauth_code_verifier')

		// redirect to the home page
		return res.status(302).redirect('/')
	} catch (e) {
		// the specific error message depends on the provider
		if (e instanceof OAuth2RequestError) {
			// invalid code
			return res.status(400).send()
		}
		return res.status(500).send()
	}
}