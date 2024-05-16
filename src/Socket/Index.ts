/** Information about all the robot sessions currently available */
export type RobotsOut = {
	/** The real robot that the user is allowed to join, can be null */
	real: RobotInfo | null
	/** The available sessions */
	sessions: RobotSession[]
}

/** Information about a real robot */
export type RobotInfo = {
	/** The name of the robot */
	name: string
	/** The address of the robot */
	address: string
	/** The port of the robot */
	port: number
	/** The vnc port of the robot */
	vnc?: number
}

/** Information about a robot session */
export type RobotSession = {
	/** The address of the session */
	address: string
	/** The type of the robot used for the session */
	type: 'virtual' | 'real'
	/** The names of users in the session */
	users: string[]
}

/** Join Session Payload. */
export type JoinSessionOut = {
	/** The address of the socket namespace this session runs on. */
	robot: RobotInfo
	/** The token needed to join the session. */
	token: string
}