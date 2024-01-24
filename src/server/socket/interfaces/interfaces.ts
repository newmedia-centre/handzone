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
}