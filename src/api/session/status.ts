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