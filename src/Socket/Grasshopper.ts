/**
 * Sends a json payload of the program to Unity to be deserialized as a program.
 */
export type GrasshopperProgramIn = {
	/** The string payload of the IProgram object to send to the server. */
	program: string
}

/**
 * Receives a json payload of the program from Unity to be deserialized as a program.
 */
export type GrasshopperProgramOut = {
	/** The string payload of the IProgram object received from the server. */
	program: string
}