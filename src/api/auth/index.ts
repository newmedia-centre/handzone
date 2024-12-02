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
import { Router } from 'express'

// import handlers
import { logout } from './logout'
import { pin } from './pin'
import { oauth } from './oauth'
import { oauthCallback } from './oauth-callback'

// create the auth router
export const auth = Router()

// add the route handlers
auth.get('/logout', logout)
auth.get('/pin', pin)
auth.get('/oauth', oauth)
auth.get('/oauth/callback', oauthCallback)