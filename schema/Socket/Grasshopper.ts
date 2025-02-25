import type { Vector3D } from './Unity'

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

/**
 * Sends an instruction to run the program on the robot.
 */
export type GrasshopperRunIn = {
	/** Whether the simulation should play or stop */
	run: boolean
}

/**
 * Sends a json payload of the meshes to Unity.
 */
export type GrasshopperMeshesIn = {
	mesh1?: MeshData
	mesh2?: MeshData
	mesh3?: MeshData
	mesh4?: MeshData
	mesh5?: MeshData
}

export type MeshData = {
	vertices: Vector3D[]
	tris: [number, number, number][]
}