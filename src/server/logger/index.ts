// import dependencies
import { createLogger, format, transports } from 'winston'
import DailyRotateFile from 'winston-daily-rotate-file'
import env from '../environment'
import fs from 'fs'

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
		new transports.Stream({
			stream: fs.createWriteStream(`${env.LOGS_PATH}/errors.log`, { flags: 'a' }),
			level: 'error'
		}),
		new DailyRotateFile({
			filename: `${env.LOGS_PATH}/%DATE%.log`,
			datePattern: 'YYYY-MM-DD',
			zippedArchive: true,
			maxSize: '20m',
			maxFiles: '7d',
			level: 'http',
		}),
	],
	exitOnError: false
})

// create child loggers
export const databaseLogger = logger.child({ entity: 'db', category: 'db', label: 'DB' })
export const socketLogger = logger.child({ entity: 'socket', category: 'socket', label: 'SOCKET' })
export const dockerLogger = logger.child({ entity: 'docker', category: 'docker', label: 'DOCKER' })
export const robotLogger = logger.child({ entity: 'robots', category: 'robot', label: 'ROBOT' })
export const webLogger = logger.child({ entity: 'web', category: 'web', label: 'WEB' })

global.logger = logger
export default logger