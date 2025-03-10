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

	/** Path to the config.json file */
	CONFIG_PATH: z.string().default('config.json'),
	LOGS_PATH: z.string().default('logs'),

	DATABASE_URL: z.string(),

	OAUTH_CLIENT_ID: z.string(),
	OAUTH_CLIENT_SECRET: z.string(),
}

// create config schema
const configSchema = z.object({
	/** Array of robot target objects */
	ROBOTS: z.array(z.object({
		name: z.string().regex(/^[a-z0-9_-]*$/),
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