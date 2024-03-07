export interface InterfacesClientToServer {
	/**
	* Set tool digital output signal level.
	*
	* @param n The number (id) of the output, integer: [0:1].
	* @param b The signal level (boolean).
	*
	* Example command: set_tool_digital_out(1, true)
	* - Example Parameters:
	*   - n is tool digital output 1.
	*   - b = true.
	*/
	'interfaces:set_tool_digital_out': (n: number, b: boolean) => void

	/**
	 * Set standard digital output signal level.
	 *
	 * @param n The number (id) of the output, integer: [0:7].
	 * @param b The signal level (boolean).
	 *
	 * Example command: set_standard_digital_out(1, true)
	 * - Example Parameters:
	 *  - n is standard digital output 1.
	 *  - b = true.
	 */
	'interfaces:set_standard_digital_out': (n: number, b: boolean) => void


	/**
	 * Calculate the inverse kinematics for a given pose.
	 *
	 * @param x Tool pose.
	 * @param qnear The initial joint position for the inverse kinematics calculation. If not provided, the current joint position is used.
	 * @param maxPositionError The maximum allowed position error. If not provided, the default value is used.
	 * @param tcp The tool center point (TCP) to use for the inverse kinematics calculation. If not provided, the default TCP is used.
	 */
	'interfaces:get_inverse_kin': (x: number[], qnear?: number[], maxPositionError?: number, tcp_offset?: string) => void
}