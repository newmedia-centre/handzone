// import dependencies
import { PrismaClient } from '@prisma/client'
import env from '../environment'

export const prisma = global.prisma || new PrismaClient()

if (env.NODE_ENV !== 'production') {
	global.prisma = prisma
}
