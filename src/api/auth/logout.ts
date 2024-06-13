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