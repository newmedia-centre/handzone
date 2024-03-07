export interface UnityClientToServer {

	/**
	 * Sends a message from one Unity client to another Unity client.
	 * @param message The message to send.
	 * @returns void
	 * @emits unity:message - The message to send.
	 */
	'unity:message': (message: string) => void


}

export interface UnityServerToClient {
	/**
	 * Sends a message from one Unity client to another Unity client.
	 * @param message The message to send.
	 * @returns void
	 */
	'unity:message': (message: string) => void
}