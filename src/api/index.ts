// import dependencies
import { Router } from 'express'

// import routers
import { auth } from './auth'

// create the api router
export const api = Router()

// set up the child routers
api.use('/auth', auth)