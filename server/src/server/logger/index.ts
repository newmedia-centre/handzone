/*
 *The MIT License (MIT)
 * Copyright (c) 2025 NewMedia Centre - Delft University of Technology
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software
 * and associated documentation files (the "Software"), to deal in the Software without restriction,
 * including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial
 * portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
 * TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
 * TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

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