// import dependencies
import { z } from 'zod'
import { prisma } from '@/server/db'
import { validateRequest } from '@/server/db/auth-next'
import { webLogger as logger } from '@/server/logger'

// handle the POST request
export async function POST(request: Request): Promise<Response> {
	// check the user session
	const { session, user } = await validateRequest()
	if (!session) {
		return new Response(null, {
			status: 403
		})
	}

	// create the request body parser
	const Data = z.object({
		robot: z.string(),
		start: z.string().datetime(),
		end: z.string().datetime(),
		session: z.string().optional()
	})

	// parse the request body
	const body = await request.json()
	const parsed = Data.safeParse(body)

	// validate the request body
	if (!parsed.success) {
		return new Response(parsed.error.message, {
			status: 400
		})
	}

	try {
		const res = await prisma.robotSessionRequest.create({
			data: {
				user: {
					connect: {
						id: user.id
					}
				},
				session: parsed.data.session ? {
					connect: {
						id: parsed.data.session
					}
				} : {
					create: {
						robot: {
							connect: {
								id: parsed.data.robot
							}
						},
						start: parsed.data.start,
						end: parsed.data.end
					}
				}
			}
		})

		return Response.json(res)
	} catch (e) {
		logger.error((e as Error).message, { error: e })
		return Response.json(e, { status: 500 })
	}
}