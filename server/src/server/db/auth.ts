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
import { Lucia } from 'lucia'
import { PrismaAdapter } from '@lucia-auth/adapter-prisma'
import { OAuth2Client } from 'oslo/oauth2'
import { parseCookies, serializeCookie } from 'oslo/cookie'
import { prisma } from '.'
import { env } from '../environment'
import { databaseLogger } from '../logger'

// import types
import type { User } from '@prisma/client'
import type { Request, Response } from 'express'

// create logger
export const logger = databaseLogger.child({ entity: 'auth', label: 'DB:AUTH' })

// connect auth to the database using prisma
const adapter = new PrismaAdapter(prisma.session, prisma.user)

// create the lucia handle
export const lucia = new Lucia(adapter, {
	sessionCookie: {
		expires: false,
		attributes: {
			secure: env.NODE_ENV === 'production'
		}
	},
	getUserAttributes: (user) => user
})

// prepare the oauth2 client
const authorizeEndpoint = env.OAUTH.authorization_endpoint
const tokenEndpoint = env.OAUTH.token_endpoint

// create the oauth2 client
export const oauth = new OAuth2Client(env.OAUTH_CLIENT_ID, authorizeEndpoint, tokenEndpoint, {
	redirectURI: `${env.URL}/api/auth/oauth/callback`
})

// get user info from the endpoint
export const getUserInfo = async (accessToken: string) => {
	const response = await fetch(env.OAUTH.userinfo_endpoint, {
		headers: {
			Authorization: `Bearer ${accessToken}`
		}
	})

	if (response.status !== 200) {
		throw new Error(response.statusText)
	}

	const json = await response.json()
	return {
		id: json[env.OAUTH.claims.id],
		name: json[env.OAUTH.claims.name],
		email: json[env.OAUTH.claims.email]
	}
}

// does not verify id tokens
export const decodeIdToken = (idToken: string) => {
	const idTokenParts = idToken.split('.')
	if (idTokenParts.length !== 3) {
		logger.error('Invalid id token')
		throw new Error('Invalid id token')
	}
	const base64UrlPayload = idTokenParts[1]!
	const payload = JSON.parse(atob(base64UrlPayload)) as {
		aud: string
		sub: string
		acr: string
		nbf: number
		scope: string
		auth_time: number
		iss: string
		exp: number
		iat: number
		jti: string
	}
	return payload
}

// validate whether the user is authenticated
export const validateApi = async (req: Request, res: Response) => {
	let sessionId: string | null = null

	// try get the session id from the authorization header
	const tokenId = lucia.readBearerToken(req.headers.authorization ?? '')
	if (tokenId) sessionId = tokenId

	// try get the session id from the cookie
	const cookieId = parseCookies(req.headers.cookie ?? '').get(lucia.sessionCookieName) ?? null
	if (cookieId) sessionId = cookieId

	// if no session id was found, return null, user is not authenticated
	if (!sessionId) {
		return {
			user: null,
			session: null
		}
	}

	const result = await lucia.validateSession(sessionId)
	// next.js throws when you attempt to set cookie when rendering page
	try {
		if (result.session && result.session.fresh) {
			const sessionCookie = lucia.createSessionCookie(result.session.id)
			res.appendHeader('Set-Cookie', serializeCookie(sessionCookie.name, sessionCookie.value, sessionCookie.attributes))
		}
		if (!result.session) {
			const sessionCookie = lucia.createBlankSessionCookie()
			res.appendHeader('Set-Cookie', serializeCookie(sessionCookie.name, sessionCookie.value, sessionCookie.attributes))
		}
	} catch {
		logger.warn('Failed to set session cookie')
	}
	return {
		user: result.user as User,
		session: result.session
	}
}