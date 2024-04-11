// import dependencies
import { generateState, generateCodeVerifier } from 'oslo/oauth2'
import { cookies } from 'next/headers'
import { oauth } from '@/server/db/auth'
import { env } from '@/server/environment'

// handle the GET request
export async function GET(): Promise<Response> {
	const state = generateState()
	const codeVerifier = generateCodeVerifier()
	const url = await oauth.createAuthorizationURL({
		state,
		scopes: ['openid'],
		codeVerifier
	})

	// set the auth state cookie
	cookies().set('oauth_state', state, {
		path: '/',
		secure: env.NODE_ENV === 'production',
		httpOnly: true,
		maxAge: 60 * 10,
		sameSite: 'lax'
	})

	// set the code verifier cookie
	cookies().set('oauth_code_verifier', codeVerifier, {
		path: '/',
		secure: env.NODE_ENV === 'production',
		httpOnly: true,
		maxAge: 60 * 10,
		sameSite: 'lax'
	})

	return Response.redirect(url)
}