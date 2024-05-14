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
	VNC_PORT: port().default(5900),
	HOSTNAME: z.string().default('localhost'),
	URL: z.string(),
	DOCKER_NETWORK: z.string(),

	DATABASE_URL: z.string(),
	OAUTH_CLIENT_ID: z.string(),
	OAUTH_CLIENT_SECRET: z.string(),

	/** Path to the config.json file */
	CONFIG_PATH: z.string().default('config.json'),
}

// create config schema
const configSchema = z.object({
	/** Array of robot target objects */
	ROBOTS: z.array(z.object({
		name: z.string(),
		port: port().default(30003),
		vnc: port().optional(),
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
	}),

	/** OAuth options */
	OAUTH: z.object({
		name: z.string().optional(),
		authorization_endpoint: z.string(),
		token_endpoint: z.string(),
		userinfo_endpoint: z.string(),
		claims: z.object({
			id: z.string(),
			name: z.string(),
			email: z.string(),
		})
	}),
})

// eslint-disable-next-line node/no-process-env
const envFile = parseEnv(process.env, envSchema)

// read the json file from the config path
const config = configSchema.parse(JSON.parse(readFileSync(envFile.CONFIG_PATH, 'utf-8')))

// export the environment
export const env = { ...envFile, ...config }
export default env