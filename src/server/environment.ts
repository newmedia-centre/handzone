// import dependencies
import 'dotenv/config'
import { readFileSync } from 'fs'
import { parseEnv, z, port } from 'znv'

// create environment schema
const envSchema = {
	/** Port that this application runs on, defaults to 3000 */
	WEB_PORT: port().default(3000),

	/** Port used to read messages from the robots */
	READ_PORT: port().default(30013),

	/** Port used to write messages to the robots */
	WRITE_PORT: port().default(30003),

	/** Current node environment, defaults to 'development' */
	NODE_ENV: z
		.enum(['development', 'production', 'test'])
		.default('development'),

	/** Path to the config.json file */
	CONFIG_PATH: z.string().default('config.json'),
}

// eslint-disable-next-line node/no-process-env
const envFile = parseEnv(process.env, envSchema)

// create config schema
const configSchema = z.object({
	/** Array of robot target objects */
	ROBOTS: z.array(z.object({
		name: z.string(),
		port: port().default(30003),
		address: z.string().ip(),
	})).optional(),

	/** Array of virtual robot target objects */
	VIRTUAL_ROBOTS: z.array(z.object({
		name: z.string(),
		port: port().default(30003),
		address: z.string().ip(),
	})).optional(),
})

// read the json file from the config path
const config = configSchema.parse(JSON.parse(readFileSync(envFile.CONFIG_PATH, 'utf-8')))

// export the environment
export const env = { ...envFile, ...config }
export default env