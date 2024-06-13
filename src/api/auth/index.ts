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