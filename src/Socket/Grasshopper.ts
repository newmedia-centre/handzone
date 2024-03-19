/**
 * Sends a json payload of the program to Unity to be deserialized as a program.
 */
export type GrasshopperProgramIn = {
	/** The message content. */
	json: string
}