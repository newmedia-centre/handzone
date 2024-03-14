// import types
import type { Vector3D } from './motion'

export type SixDofPosition = {
	position: Vector3D
	rotation: Vector3D
}

export type PlayerData = {
	id: string
	hmd: SixDofPosition
	left: SixDofPosition
	right: SixDofPosition
	name: string
	color: string
}

export type UnityMessageIn = {
	message: string
}

export type UnityPlayerIn = {
	hmd: SixDofPosition
	left: SixDofPosition
	right: SixDofPosition
}

export type UnityPendantIn = {
	position: Vector3D
	rotation: Vector3D
}

export type UnityMessageOut = {
	message: string
}

export type UnityPlayerOut = {
	players: PlayerData[]
}

export type UnityPendantOut = {
	owner: string
	position: Vector3D
	rotation: Vector3D
}

export interface UnityClientToServer {

	/**
	 * Sends a message from one Unity client to another Unity client.
	 * @param message The message to send.
	 * @returns void
	 * @emits unity:message - The message to send.
	 */
	'unity:message': (payload: UnityMessageIn) => void

	/**
	 * Allows the Unity client to send its current position
	 * @param payload The position data.
	 * @returns void
	 */
	'unity:player': (payload: UnityPlayerIn) => void

	/**
		 * Grabs ownership and sends the position of the pendant.
		 * @param payload The pendant position data.
		 * @returns void
		 */
	'unity:pendant': (payload: UnityPendantIn) => void
}

export interface UnityServerToClient {
	/**
	 * Sends a message from one Unity client to another Unity client.
	 * @param message The message to send.
	 * @emits unity:message - The message to send.
	 * @returns void
	 */
	'unity:message': (payload: UnityMessageOut) => void

	/**
	 * Sends an array of all the Unity clients' positions.
	 * @param payload The position data.
	 * @returns void
	 */
	'unity:players': (payload: UnityPlayerOut) => void

	/**
		 * Sends the current position of the pendant.
		 * @param payload The pendant position data.
		 * @emits unity:pendant - The message to send.
		 * @returns void
		 */
	'unity:pendant': (payload: UnityPendantOut) => void
}