// import dependencies
import { Router, json } from 'express'

// import handlers
import { logout } from './logout'
import { postPin, putPin } from './pin'
import { oauth } from './oauth'
import { oauthCallback } from './oauth-callback'

// create the auth router
export const auth = Router()

// set up middleware
auth.use(json())

// add the route handlers
auth.get('/logout', logout)
auth.post('/pin', postPin)
auth.put('/pin', putPin)
auth.get('/oauth', oauth)
auth.get('/oauth/callback', oauthCallback)