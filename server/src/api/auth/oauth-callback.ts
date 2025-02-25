/*
 *The MIT License (MIT)
 * Copyright (c) 2025 NewMedia Centre - Delft University of Technology
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

// import dependencies
import { OAuth2RequestError } from 'oslo/oauth2'
import { parseCookies, serializeCookie } from 'oslo/cookie'
import { oauth, lucia, getUserInfo, decodeIdToken } from '@/server/db/auth'
import { prisma } from '@/server/db'
import { env } from '@/server/environment'

// import types
import type { Request, Response } from 'express'
import type { TokenResponseBody } from 'oslo/oauth2'

// create the oauth route
export const oauthCallback = async (req: Request, res: Response) => {
	const code = req.query.code?.toString() ?? null
	const state = req.query.state?.toString() ?? null

	const storedState = parseCookies(req.headers.cookie ?? '').get('oauth_state') ?? null
	const storedCodeVerifier = parseCookies(req.headers.cookie ?? '').get('oauth_code_verifier') ?? null

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
			res.appendHeader('Set-Cookie', serializeCookie(sessionCookie.name, sessionCookie.value, sessionCookie.attributes))
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
			res.appendHeader('Set-Cookie', serializeCookie(sessionCookie.name, sessionCookie.value, sessionCookie.attributes))
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