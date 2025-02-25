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