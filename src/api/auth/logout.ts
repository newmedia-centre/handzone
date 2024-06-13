// import dependencies
import { lucia, validateApi } from '@/server/db/auth'

// import types
import type { Request, Response } from 'express'

// create the logout route
export const logout = async (req: Request, res: Response) => {
	const { session } = await validateApi(req, res)
	if (!session) {
		return res.redirect('/about')
	}

	await lucia.invalidateSession(session.id)

	const sessionCookie = lucia.createBlankSessionCookie()
	res.cookie(sessionCookie.name, sessionCookie.value, sessionCookie.attributes)
	return res.redirect('/about')
}