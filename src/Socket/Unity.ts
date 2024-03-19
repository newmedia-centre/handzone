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
 * Represents a message from Unity.
 */
export type UnityMessageIn = {
	/** The message content. */
	message: string
}

/**
 * Represents a player's position in Unity.
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
 * Represents a pendant's position in Unity.
 */
export type UnityPendantIn = {
	/** The pendant's position. */
	position: Vector3D
	/** The pendant's rotation. */
	rotation: Vector3D
}

/**
 * Represents a message to Unity.
 */
export type UnityMessageOut = {
	/** The message content. */
	message: string
}

/**
 * Represents the data of players in Unity.
 */
export type UnityPlayerOut = {
	/** An array of player data. */
	players: PlayerData[]
}

/**
 * Represents a pendant's position in Unity.
 */
export type UnityPendantOut = {
	/** The owner of the pendant. */
	owner: string,
	/** The pendant's position. */
	position: Vector3D,
	/** The pendant's rotation. */
	rotation: Vector3D
}