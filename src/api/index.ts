// import dependencies
import { Router } from 'express'

// import routers
import { auth } from './auth'
import { session } from './session'
import { robot } from './robot'

// create the api router
export const api = Router()

// set up the child routers
api.use('/auth', auth)
api.use('/session', session)
api.use('/robot', robot)