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