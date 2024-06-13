// import dependencies
import { Router, json } from 'express'

// import handlers
import { createSession, deleteSession } from './manage'
import { updateStatus, deleteStatus } from './status'

// create the session router
export const session = Router()

session.use(json())

// add the route handlers
session.post('/', createSession)
session.delete('/:id', deleteSession)
session.put('/:id/status', updateStatus)
session.delete('/:id/status', deleteStatus)