import type { Vector3D } from './Unity'

/**
 * Sends a json payload of the program to Unity to be deserialized as a program.
 */
export type GrasshopperProgramIn = {
	/** The string payload of the IProgram object to send to the server. */
	program: string
	/** If true, program is base64(gzip(json)) */
	compressed?: boolean
	/** Minimal joint-space waypoints (radians) to play on the client. If provided, clients may ignore 'program'. */
	joints?: number[][]
	/** Optional per-waypoint durations in seconds (between consecutive waypoints). If provided, takes precedence over dt. */
	times?: number[]
	/** Optional timestep between joint waypoints in seconds. */
	dt?: number
	/** Units for joints (default 'rad'). */
	units?: 'rad' | 'deg'
	/** If true, consumers should treat this as a hard reload of the current program */
	reload?: boolean
	/** A monotonically changing identifier to force cache invalidation on consumers (e.g., ISO timestamp or GUID) */
	revision?: string
}

/**
 * Receives a json payload of the program from Unity to be deserialized as a program.
 */
export type GrasshopperProgramOut = {
	/** The string payload of the IProgram object received from the server. */
	program: string
	/** Minimal joint-space waypoints (radians) broadcast to clients. */
	joints?: number[][]
	/** Optional per-waypoint durations in seconds (between consecutive waypoints). If provided, takes precedence over dt. */
	times?: number[]
	/** Optional timestep between joint waypoints in seconds. */
	dt?: number
	/** Units for joints (default 'rad'). */
	units?: 'rad' | 'deg'
	/** If true, consumers should treat this as a hard reload of the current program */
	reload?: boolean
	/** A monotonically changing identifier to force cache invalidation on consumers */
	revision?: string
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