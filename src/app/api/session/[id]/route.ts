// import dependencies
import { prisma } from '@/server/db'
import { validateRequest } from '@/server/db/auth'
import { webLogger as logger } from '@/server/logger'

// handle the DELETE request
export async function DELETE(_: Request, { params }: { params: { id: string } }): Promise<Response> {

	// check the user session
	const { session, user } = await validateRequest()
	if (!session || !user?.admin) {
		return new Response(null, {
			status: 403
		})
	}

	try {
		const res = await prisma.robotSession.delete({
			where: {
				id: params.id
			}
		})

		return Response.json(res)
	} catch (e) {
		logger.error((e as Error).message, { error: e })
		return Response.json(e, { status: 500 })
	}
}