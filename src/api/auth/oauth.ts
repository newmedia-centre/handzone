// import dependencies
import { generateState, generateCodeVerifier } from 'oslo/oauth2'
import { serializeCookie } from 'oslo/cookie'
import { oauth as luciaOauth } from '@/server/db/auth'
import { env } from '@/server/environment'

// import types
import type { Request, Response } from 'express'

// create the oauth route
export const oauth = async (req: Request, res: Response) => {
	const state = generateState()
	const codeVerifier = generateCodeVerifier()
	const url = await luciaOauth.createAuthorizationURL({
		state,
		scopes: ['openid'],
		codeVerifier
	})

	// set the auth state cookie
	res.appendHeader('Set-Cookie', serializeCookie('oauth_state', state, {
		path: '/',
		secure: env.NODE_ENV === 'production',
		httpOnly: true,
		maxAge: 60 * 10,
		sameSite: 'lax'
	}))

	// set the code verifier cookie
	res.appendHeader('Set-Cookie', serializeCookie('oauth_code_verifier', codeVerifier, {
		path: '/',
		secure: env.NODE_ENV === 'production',
		httpOnly: true,
		maxAge: 60 * 10,
		sameSite: 'lax'
	}))

	return res.redirect(url.toString())
}