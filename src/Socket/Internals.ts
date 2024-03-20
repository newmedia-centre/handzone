/** Calculate the inverse kinematics for a given pose. */
export type InternalsGetInverseKinIn = {
	/** Tool pose. */
	x: number[]
	/** The initial joint position for the inverse kinematics calculation. If not provided, the current joint position is used. */
	qnear?: number[]
	/** The maximum allowed position error. If not provided, the default value is used. */
	maxPositionError?: number
	/** The tool center point (TCP) to use for the inverse kinematics calculation. If not provided, the default TCP is used. */
	tcp_offset?: string
}

export type InternalsGetInverseKinCallback = {
	/** The inverse kinematics positions */
	ik: number[]
}