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
import { validateApi } from '@/server/db/auth'
import { generatePin } from '@/server/db/pin'

// import types
import type { Request, Response } from 'express'

// create the pin route
export const pin = async (req: Request, res: Response) => {
	// check the user session
	const { session, user } = await validateApi(req, res)
	if (!session) {
		res.status(403)
		return res.send()
	}

	// generate and send the pin
	const pin = await generatePin(user)
	res.status(200)
	res.send(pin)
}