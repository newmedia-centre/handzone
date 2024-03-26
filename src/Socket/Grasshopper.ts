/**
 * Sends a json payload of the program to Unity to be deserialized as a program.
 */
export type GrasshopperProgramIn = {
	/** The the json payload of the IProgram object to send to the server. */
	json: string
}

/**
 * Receives a json payload of the program from Unity to be deserialized as a program.
 */
export type GrasshopperProgramOut = {
	/** The json payload of the IProgram object received from the server. */
	json: string
}