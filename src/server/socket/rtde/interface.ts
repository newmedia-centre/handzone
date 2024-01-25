// import types
import type { Vector3D, Vector6D } from '../interface'

export type RTDEData = {
	timestamp: number
}

// declare socket.io interfaces
export interface RTDEServerToClient {
	/**
	 * The raw data from the robot, encoded as a base64 string.
	 */
	'rtde:raw': (data: string) => void

	/** The parsed data from the robot */
	'rtde:data': (data: RTDEData) => void
}

export interface RTDEClientToServer {
	/**
	* Determines whether to change the speed slider with this input.
	* @param value 0 = Don't change speed slider with this input, 1 = Use speed_slider_fraction to set speed slider value
	*/
	'rtde:speed_slider_mask': (value: number) => void

	/**
	* Sets a new speed slider value.
	* @param value New speed slider value
	*/
	'rtde:speed_slider_fraction': (value: number) => void

	/**
	* Sets the standard digital output bit mask.
	* @param value Standard digital output bit mask
	*/
	'rtde:standard_digital_output_mask': (value: number) => void

	/**
	* Sets the configurable digital output bit mask.
	* @param value Configurable digital output bit mask
	*/
	'rtde:configurable_digital_output_mask': (value: number) => void

	/**
	* Sets the standard digital outputs.
	* @param value Standard digital outputs
	*/
	'rtde:standard_digital_output': (value: number) => void

	/**
	* Sets the configurable digital outputs.
	* @param value Configurable digital outputs
	*/
	'rtde:configurable_digital_output': (value: number) => void

	/**
	* Sets the standard analog output mask.
	* @param value Bits 0-1: standard_analog_output_0 | standard_analog_output_1
	*/
	'rtde:standard_analog_output_mask': (value: number) => void

	/**
	* Sets the output domain for standard analog outputs.
	* @param value Output domain {0=current[mA], 1=voltage[V]} Bits 0-1: standard_analog_output_0 | standard_analog_output_1
	*/
	'rtde:standard_analog_output_type': (value: number) => void

	/**
	* Sets the standard analog output 0 (ratio) [0..1].
	* @param value Standard analog output 0 (ratio) [0..1]
	*/
	'rtde:standard_analog_output_0': (value: number) => void

	/**
	* Sets the standard analog output 1 (ratio) [0..1].
	* @param value Standard analog output 1 (ratio) [0..1]
	*/
	'rtde:standard_analog_output_1': (value: number) => void

	/**
	* Sets general purpose bits.
	* @param value This range of the boolean input registers is reserved for FieldBus/PLC interface usage.
	*/
	'rtde:input_bit_registers0_to_31': (value: number) => void

	/**
	* Sets general purpose bits.
	* @param value This range of the boolean input registers is reserved for FieldBus/PLC interface usage.
	*/
	'rtde:input_bit_registers32_to_63': (value: number) => void

	/**
	* Sets a specific general purpose bit.
	* @param value The boolean value to set
	* @param index The index of the bit to set (64 general purpose bits index: [64..127])
	*/
	'rtde:input_bit_register': (value: boolean, index: number) => void

	/**
	* Sets a specific general purpose integer.
	* @param value The integer value to set
	* @param index The index of the integer to set (index range: specify the range here)
	*/
	'rtde:input_int_register': (value: number, index: number) => void

	/**
	* Sets a specific general purpose double.
	* @param value The double value to set
	* @param index The index of the double to set (index range: specify the range here)
	*/
	'rtde:input_double_register': (value: number, index: number) => void

	/**
	* Sets the external force-torque values.
	* This command allows for setting the external force and torque applied to the robot.
	* @param value A Vector6D object representing the forces and torques.
	*/
	'rtde:external_force_torque': (value: Vector6D) => void
}