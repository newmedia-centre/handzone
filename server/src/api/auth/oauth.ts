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