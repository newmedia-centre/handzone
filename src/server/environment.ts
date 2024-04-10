// import dependencies
import 'dotenv/config'
import { readFileSync } from 'fs'
import { parseEnv, z, port } from 'znv'

// create environment schema
const envSchema = {
	/** Node Env */
	NODE_ENV: z.enum(['development', 'production']).default('development'),

	/** Port that this application runs on, defaults to 3000 */
	PORT: port().default(3000),
	TCP_PORT: port().default(4000),
	HOSTNAME: z.string().default('localhost'),
	DOCKER_NETWORK: z.string(),

	DATABASE_URL: z.string(),
	CLIENT_ID: z.string(),
	CLIENT_SECRET: z.string(),

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
		address: z.string(),
		camera: z.array(z.object({
			name: z.string(),
			address: z.string(),
		})).default([]),
	})).default([]),

	/** Virtual polyscope options */
	DOCKER: z.object({
		OPTIONS: z.any(),
		MAX_VIRTUAL: z.number().default(3),
	})
})

// read the json file from the config path
const config = configSchema.parse(JSON.parse(readFileSync(envFile.CONFIG_PATH, 'utf-8')))

// export the environment
export const env = { ...envFile, ...config }
export default env