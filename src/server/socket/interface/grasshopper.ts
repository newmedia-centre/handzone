export interface GrasshopperClientToServer {

	/**
	 * Sends a json payload of the program to Unity to be deserialized as a program.
	 * @param json The json payload of the IProgram object to send.
	 * @returns void
	 * @emits grasshopper:program - The the json payload of the IProgram object to send.
	 */
	'grasshopper:program': (json: string) => void


}

export interface GrasshopperServerToClient {
	/**
	 * Sends a json payload of the program to Unity to be deserialized as a program.
	 * @param json The json payload of the IProgram object to send.
	 * @returns void
	 */
	'grasshopper:program': (json: string) => void
}