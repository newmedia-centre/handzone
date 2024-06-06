// import dependencies
import { z } from 'zod'
import { prisma } from '@/server/db'
import { validateRequest } from '@/server/db/auth'
import { webLogger as logger } from '@/server/logger'
import { RequestStatus } from '@prisma/client'

// handle the PUT request
export async function PUT(request: Request, { params }: { params: { id: string } }): Promise<Response> {

	// check the user session
	const { session } = await validateRequest()
	if (!session) {
		return new Response(null, {
			status: 403
		})
	}

	// create the request body parser
	const Data = z.object({
		user: z.string(),
		status: z.enum([RequestStatus.REQUESTED, RequestStatus.ACCEPTED, RequestStatus.REJECTED, RequestStatus.AVAILABLE, RequestStatus.CANCELLED])
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
		const res = await prisma.robotSessionRequest.update({
			where: {
				sessionId_userId: {
					sessionId: params.id,
					userId: parsed.data.user
				}
			},
			data: {
				status: parsed.data.status
			}
		})

		return Response.json(res)
	} catch (e) {
		logger.error((e as Error).message, { error: e })
		return Response.json(e, { status: 500 })
	}
}