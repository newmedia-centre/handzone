// import dependencies
import { Router } from 'express'
import cookieParser from 'cookie-parser'

// import routers
import { auth } from './auth'

// create the api router
export const api = Router()
api.use(cookieParser())

// set up the child routers
api.use('/auth', auth)