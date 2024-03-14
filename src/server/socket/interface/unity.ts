// import types
import type { Vector6D } from '@/types/Socket/Vector6d'

export type PositionDataIn = {
	hmd: Vector6D
	left: Vector6D
	right: Vector6D
}

export type PositionDataOut = {
	id: string
	hmd: Vector6D
	left: Vector6D
	right: Vector6D
	name: string
	color: string
}

export interface UnityClientToServer {

	/**
	 * Sends a message from one Unity client to another Unity client.
	 * @param message The message to send.
	 * @returns void
	 * @emits unity:message - The message to send.
	 */
	'unity:message': (message: string) => void

	/**
	 * Allows the Unity client to send its current position
	 * @param payload The position data.
	 * @returns void
	 */
	'unity:position': (payload: PositionDataIn) => void

	/**
		 * Grabs ownership and sends the position of the pendant.
		 * @param payload The pendant position data.
		 * @returns void
		 */
	'unity:pendant': (payload: Vector6D) => void
}

export interface UnityServerToClient {
	/**
	 * Sends a message from one Unity client to another Unity client.
	 * @param message The message to send.
	 * @emits unity:message - The message to send.
	 * @returns void
	 */
	'unity:message': (message: string) => void

	/**
	 * Sends an array of all the Unity clients' positions.
	 * @param payload The position data.
	 * @returns void
	 */
	'unity:position': (payload: PositionDataOut[]) => void

	/**
		 * Sends the current position of the pendant.
		 * @param payload The pendant position data.
		 * @emits unity:pendant - The message to send.
		 * @returns void
		 */
	'unity:pendant': (payload: { owner: string, position: Vector6D }) => void
}