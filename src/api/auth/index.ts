// import dependencies
import { Router } from 'express'

// import handlers
import { logout } from './logout'
import { pin } from './pin'
import { oauth } from './oauth'
import { oauthCallback } from './oauth-callback'

// create the auth router
export const auth = Router()

// create the logout route
auth.get('/logout', logout)

// create the pin route
auth.get('/pin', pin)

// create the oauth route
auth.get('/oauth', oauth)

// create the oauth callback route
auth.get('/oauth/callback', oauthCallback)