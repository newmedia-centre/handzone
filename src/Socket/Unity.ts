/** Represents a 6D vector consisting of three force components and three torque components. */
export type Vector6D = {
	/** X-axis */
	x: number
	/** Y-axis */
	y: number
	/** Z-axis */
	z: number
	/** U-axis */
	u: number
	/** V-axis */
	v: number
	/** W-axis */
	w: number
}

/** Represents a 3D vector consisting of three components */
export type Vector3D = {
	/** X-axis */
	x: number
	/** Y-axis */
	y: number
	/** Z-axis */
	z: number
}

/** Represents a 2D vector consisting of two components */
export type Vector2D = {
	/** X-axis */
	x: number
	/** Y-axis */
	y: number
}


/**
 * Represents a 6 degrees of freedom position with position and rotation vectors.
 */
export type SixDofPosition = {
	/** The position vector. */
	position: Vector3D
	/** The rotation vector. */
	rotation: Vector3D
}

/**
 * Represents the data of a player.
 */
export type PlayerData = {
	/** The player's ID. */
	id: string
	/** The player's head-mounted display position. */
	hmd: SixDofPosition
	/** The player's left hand position. */
	left: SixDofPosition
	/** The player's right hand position. */
	right: SixDofPosition
	/** The player's cursor position on the pendant */
	cursor: Vector2D
	/** The player's name. */
	name: string
	/** The player's color. */
	color: string
}

/**
 * Sends a message from one Unity client to another Unity client.
 */
export type UnityMessageIn = {
	/** The message content. */
	message: string
}

/**
 * Allows the Unity client to send its current position
 */
export type UnityPlayerIn = {
	/** The player's head-mounted display position. */
	hmd: SixDofPosition
	/** The player's left hand position. */
	left: SixDofPosition
	/** The player's right hand position. */
	right: SixDofPosition
	/** The player's cursor position on the pendant */
	cursor: Vector2D
}

/**
 * Sends a message from one Unity client to another Unity client.
 */
export type UnityMessageOut = {
	/** The message content. */
	message: string
}

/**
 * Sends an array of all the Unity clients' positions.
 */
export type UnityPlayersOut = {
	/** An array of player data. */
	players: PlayerData[]
}

/**
 * Sends the current ownership of the pendant.
 */
export type UnityPendantOut = {
	/** The player's ID of the owner of the pendant. */
	owner: string
}

/**
 * Sends an instruction to run the program on the robot.
 */
export type UnityRunIn = {
	/** Whether the simulation should play or stop */
	run: boolean
}