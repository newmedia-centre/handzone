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
import { cookies } from 'next/headers'
import { prisma } from '.'
import { env } from '../environment'
import { databaseLogger } from '../logger'

// import types
import type { User } from '@prisma/client'

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

// validate whether the user is authenticated
export const validateRequest = async (authorizationHeader?: string) => {
	let sessionId: string | null = null

	// try get the session id from the authorization header
	const tokenId = lucia.readBearerToken(authorizationHeader ?? '')
	if (tokenId) sessionId = tokenId

	// try get the session id from the cookie
	const cookieId = cookies().get(lucia.sessionCookieName)?.value
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
			cookies().set(sessionCookie.name, sessionCookie.value, sessionCookie.attributes)
		}
		if (!result.session) {
			const sessionCookie = lucia.createBlankSessionCookie()
			cookies().set(sessionCookie.name, sessionCookie.value, sessionCookie.attributes)
		}
	} catch {
		logger.warn('Failed to set session cookie')
	}
	return {
		user: result.user as User,
		session: result.session
	}
}