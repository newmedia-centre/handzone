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
import { lucia, validateApi } from '@/server/db/auth'
import { serializeCookie } from 'oslo/cookie'

// import types
import type { Request, Response } from 'express'

// create the logout route
export const logout = async (req: Request, res: Response) => {
	const { session } = await validateApi(req, res)
	console.log('logout attempt without session', session)
	if (!session) {
		return res.redirect('/about')
	}

	await lucia.invalidateSession(session.id)

	const sessionCookie = lucia.createBlankSessionCookie()
	res.appendHeader('Set-Cookie', serializeCookie(sessionCookie.name, sessionCookie.value, sessionCookie.attributes))
	return res.redirect('/about')
}