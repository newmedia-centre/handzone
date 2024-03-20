/** Set tool digital output signal level. */
export type InterfacesSetToolDigitalOutIn = {
	/** The number (id) of the output, integer: [0:1]. */
	n: number
	/** The signal level (boolean). */
	b: boolean
}

/** Set standard digital output signal level. */
export type InterfacesSetStandardDigitalOutIn = {
	/** The number (id) of the output, integer: [0:7]. */
	n: number
	/** The signal level (boolean). */
	b: boolean
}

/** Calculate the inverse kinematics for a given pose. */
export type InterfacesGetInverseKinIn = {
	/** Tool pose. */
	x: number[]
	/** The initial joint position for the inverse kinematics calculation. If not provided, the current joint position is used. */
	qnear?: number[]
	/** The maximum allowed position error. If not provided, the default value is used. */
	maxPositionError?: number
	/** The tool center point (TCP) to use for the inverse kinematics calculation. If not provided, the default TCP is used. */
	tcp_offset?: string
}

export type InterfacesGetInverseKinCallback = {
	/** The inverse kinematics positions */
	ik: number[]
}