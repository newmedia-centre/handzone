// import dependencies
import { createLogger, format, transports } from 'winston'
import path from 'path'
import env from '../environment'

// create logger
export const logger = global.logger || createLogger({
	format: format.combine(
		format.timestamp({ format: 'YYYY-MM-DD HH:mm:ss.SSS' }),
		format.errors({ stack: true }),
		format.json()
	),
	transports: [
		new transports.Console({
			format: format.combine(
				format.colorize(),
				format.timestamp({ format: 'YYYY-MM-DD HH:mm:ss.SSS' }),
				format.errors({ stack: true }),
				format.printf(({ level, message, timestamp, stack }) => {
					return `${timestamp} ${level}: ${stack || message}`
				})
			),
		}),
		new transports.File({
			filename: path.resolve(process.cwd(), 'logs', `${Date.now()}.errors.log`),
			level: 'error'
		}),
		new transports.File({
			filename: path.resolve(process.cwd(), 'logs', `${Date.now()}.log`)
		}),
	],
})

if (env.NODE_ENV !== 'production') {
	global.logger = logger
}
