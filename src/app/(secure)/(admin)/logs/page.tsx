// import dependencies
import fs from 'fs'
import path from 'path'
import env from '@/server/environment'

// import components
import { LogTable } from './table'

// import types
import type { LogData } from './table'

export default function Page() {
	// read the logs
	const errors = fs.readFileSync(path.resolve(env.LOGS_PATH, 'errors.log'), 'utf-8')
		.split('\n')
		.filter(x => x.length > 0)
		.map(x => ({ ...JSON.parse(x), raw: x }) as LogData)

	return (
		<LogTable data={errors} />
	)
}
