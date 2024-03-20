import type { Vector3D } from './Motion'
/**
 * Represents a 6 degrees of freedom position with position and rotation vectors.
 */
export type SixDofPosition = {
	/** The position vector. */
	position: Vector3D,
	/** The rotation vector. */
	rotation: Vector3D
}

/**
 * Represents the data of a player.
 */
export type PlayerData = {
	/** The player's ID. */
	id: string,
	/** The player's head-mounted display position. */
	hmd: SixDofPosition,
	/** The player's left hand position. */
	left: SixDofPosition,
	/** The player's right hand position. */
	right: SixDofPosition,
	/** The player's name. */
	name: string,
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
}

/**
 * Grabs ownership and sends the position of the pendant.
 */
export type UnityPendantIn = {
	/** The pendant's position. */
	position: Vector3D
	/** The pendant's rotation. */
	rotation: Vector3D
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
 * Sends the current position of the pendant.
 */
export type UnityPendantOut = {
	/** The owner of the pendant. */
	owner: string,
	/** The pendant's position. */
	position: Vector3D,
	/** The pendant's rotation. */
	rotation: Vector3D
}