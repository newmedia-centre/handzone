// import dependencies
import { createLogger, format, transports } from 'winston'
import path from 'path'

// create logger
export const logger = global.logger ?? createLogger({
	format: format.combine(
		format.timestamp(),
		format.errors({ stack: true }),
		format.json()
	),
	transports: [
		new transports.Console({
			format: format.combine(
				format.colorize(),
				format.errors({ stack: true }),
				format.printf(({ level, message, stack, label, meta }) => {
					return `[${label}] ${level}: ${stack || message}${meta ? (', ' + JSON.stringify(meta, null, 2)) : ''}`
				})
			),
		}),
		new transports.File({
			filename: path.resolve(process.cwd(), 'logs', `${new Date(Date.now()).toISOString()}.errors.log`),
			level: 'error'
		}),
		new transports.File({
			filename: path.resolve(process.cwd(), 'logs', `${new Date(Date.now()).toISOString()}.log`)
		}),
	],
	exitOnError: false
})

// create child loggers
export const databaseLogger = logger.child({ entity: 'db', category: 'db', label: 'DB' })
export const socketLogger = logger.child({ entity: 'socket', category: 'socket', label: 'SOCKET' })
export const dockerLogger = logger.child({ entity: 'docker', category: 'docker', label: 'DOCKER' })
export const robotLogger = logger.child({ entity: 'robot', category: 'robot', label: 'ROBOT' })
export const webLogger = logger.child({ entity: 'web', category: 'web', label: 'WEB' })

// fix global instancing in production // TODO
global.logger = logger
export default logger