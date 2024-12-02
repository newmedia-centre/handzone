/*
 * Copyright (c) 2024 NewMedia Centre - Delft University of Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the 'License');
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *    http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an 'AS IS' BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
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