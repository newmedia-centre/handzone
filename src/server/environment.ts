// import dependencies
import 'dotenv/config'
import { parseEnv, z, port } from 'znv'

// create environment schema
const envSchema = {
	/** Port that this application runs on, defaults to 3000 */
	PORT: port().default(3000),

	/** Current node environment, defaults to 'development' */
	NODE_ENV: z
		.enum(['development', 'production', 'test'])
		.default('development'),

	/** Array of robot urls to target for control */
	ROBOTS: z.string().transform(x => x.split(',')).optional(),

	/** Array of virtual robot urls to target for control */
	VIRTUAL_ROBOTS: z.string().transform(x => x.split(',')).optional(),
}

// export the environment
export const env = parseEnv(process.env, envSchema)
export default env