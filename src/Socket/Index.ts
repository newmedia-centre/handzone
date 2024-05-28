/** Information about all the robot sessions currently available */
export type SessionsOut = {
	/** the capacity for new virtual robots */
	capacity: number
	/** The available virtual robot sessions */
	sessions: RobotSession[]
}

/** Information about a real robot */
export type RobotInfo = {
	/** The name of the robot */
	name: string
	/** The address of the robot */
	address: string
}

/** Information about a robot session */
export type RobotSession = {
	/** The name of the robot */
	name: string
	/** The address of the robot */
	address: string
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