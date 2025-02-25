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
import { z } from 'zod'
import { prisma } from '@/server/db'
import { validateApi } from '@/server/db/auth'
import { webLogger as logger } from '@/server/logger'
import { RequestStatus } from '@prisma/client'

// import types
import type { Request, Response } from 'express'

// create the post route
export const updateStatus = async (req: Request, res: Response) => {

	// check the user session
	const { session } = await validateApi(req, res)
	if (!session) {
		return res.status(403).send()
	}

	// create the request body parser
	const Data = z.object({
		user: z.string(),
		status: z.enum([RequestStatus.REQUESTED, RequestStatus.ACCEPTED, RequestStatus.REJECTED])
	})

	// parse the request body
	const body = req.body
	const parsed = Data.safeParse(body)

	// validate the request body
	if (!parsed.success) {
		return res.status(400).send(parsed.error.message)
	}

	// check if an id was provided
	if (!req.params.id) {
		return res.status(400).send()
	}

	try {
		const request = await prisma.robotSessionRequest.update({
			where: {
				sessionId_userId: {
					sessionId: req.params.id,
					userId: parsed.data.user
				}
			},
			data: {
				status: parsed.data.status
			}
		})

		return res.json(request)
	} catch (e) {
		logger.error((e as Error).message, { error: e })
		return res.status(500).json(e)
	}
}

// create the delete route
export const deleteStatus = async (req: Request, res: Response) => {

	// check the user session
	const { session, user } = await validateApi(req, res)
	if (!session) {
		return res.status(403).send()
	}

	// check if an id was provided
	if (!req.params.id) {
		return res.status(400).send()
	}

	try {
		const request = await prisma.robotSessionRequest.delete({
			where: {
				sessionId_userId: {
					sessionId: req.params.id,
					userId: user.id
				}
			}
		})

		await prisma.robotSession.deleteMany({
			where: {
				id: req.params.id,
				requests: {
					none: {}
				}
			}
		})

		return res.json(request)
	} catch (e) {
		logger.error((e as Error).message, { error: e })
		return res.status(500).json(e)
	}
}