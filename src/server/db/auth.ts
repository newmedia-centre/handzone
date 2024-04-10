// import dependencies
import { Lucia } from 'lucia'
import { PrismaAdapter } from '@lucia-auth/adapter-prisma'
import { OAuth2Client } from 'oslo/oauth2'
import { cookies } from 'next/headers'
import { cache } from 'react'
import { prisma } from '.'
import { env } from '../environment'

// import types
import type { User } from '@prisma/client'

// define the user returned by SURFConext
export type SurfUserResponse = {
	sub: string,
	uids: string[],
	schac_personal_unique_code: string[],
	name: string,
	email: string,
	email_verified: boolean,
	eduperson_affiliation: ('student' | 'pre-student' | 'employee' | 'faculty' | 'member' | 'affiliate')[]
}

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

// prepare the SURFConext oauth2 client
const authorizeEndpoint = 'https://connect.test.surfconext.nl/oidc/authorize'
const tokenEndpoint = 'https://connect.test.surfconext.nl/oidc/token'

// create the SURFConext oauth2 client
export const surf = new OAuth2Client(env.CLIENT_ID, authorizeEndpoint, tokenEndpoint, {
	redirectURI: 'http://localhost:3000/api/auth/surf/callback'
})

// get user info from the SURFConext endpoint
export const getUserInfo = async (accessToken: string) => {
	const response = await fetch('https://connect.test.surfconext.nl/oidc/userinfo', {
		headers: {
			Authorization: `Bearer ${accessToken}`
		}
	})

	if (response.status !== 200) {
		throw new Error(response.statusText)
	}

	return await response.json() as SurfUserResponse
}

// does not verify id tokens
export const decodeIdToken = (idToken: string) => {
	const idTokenParts = idToken.split('.')
	if (idTokenParts.length !== 3) throw new Error('')
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
export const validateRequest = cache(
	async () => {
		const sessionId = cookies().get(lucia.sessionCookieName)?.value ?? null
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
		} catch { }
		return {
			user: result.user as User,
			session: result.session
		}
	}
)