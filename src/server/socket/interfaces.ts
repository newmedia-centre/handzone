// import types
import type { Socket as BareSocket } from 'socket.io'

/** Represents a 6D vector consisting of three force components and three torque components. */
export type Vector6D = {
	/** force along the X-axis */
	fx: number
	/** force along the Y-axis */
	fy: number
	/** force along the Z-axis */
	fz: number
	/** torque around the X-axis */
	tx: number
	/** torque around the Y-axis */
	ty: number
	/** torque around the Z-axis */
	tz: number
};

/** Represents a 3D vector consisting of three components */
export type Vector3D = {
	/** X-axis */
	x: number
	/** Y-axis */
	y: number
	/** Z-axis */
	z: number
};

// declare socket.io interfaces
export interface ServerToClientEvents {
	/**
	 * Time elapsed since the controller was started [s].
	 * @param value Time elapsed (in seconds)
	 */
	timestamp: (value: number) => void;

	/**
	 * Target joint positions.
	 * @param value Target joint positions (Vector6D object)
	 */
	target_q: (value: Vector6D) => void;

	/**
	 * Target joint velocities.
	 * @param value Target joint velocities (Vector6D object)
	 */
	target_qd: (value: Vector6D) => void;

	/**
	 * Target joint accelerations.
	 * @param value Target joint accelerations (Vector6D object)
	 */
	target_qdd: (value: Vector6D) => void;

	/**
	 * Target joint currents.
	 * @param value Target joint currents (Vector6D object)
	 */
	target_current: (value: Vector6D) => void;

	/**
	 * Target joint moments (torques).
	 * @param value Target joint moments (torques) (Vector6D object)
	 */
	target_moment: (value: Vector6D) => void;

	/**
	 * Actual joint positions.
	 * @param value Actual joint positions (Vector6D object)
	 */
	actual_q: (value: Vector6D) => void;

	/**
	 * Actual joint velocities.
	 * @param value Actual joint velocities (Vector6D object)
	 */
	actual_qd: (value: Vector6D) => void;

	/**
	 * Actual joint currents.
	 * @param value Actual joint currents (Vector6D object)
	 */
	actual_current: (value: Vector6D) => void;

	/**
	 * Actual current window.
	 * @param value Actual current window (Vector6D object)
	 */
	actual_current_window: (value: Vector6D) => void;

	/**
	 * Joint control currents.
	 * @param value Joint control currents (Vector6D object)
	 */
	joint_control_output: (value: Vector6D) => void;

	/**
	 * Actual Cartesian coordinates of the tool.
	 * @param value Actual Cartesian coordinates (Vector6D object: x, y, z, rx, ry, rz)
	 */
	actual_TCP_pose: (value: Vector6D) => void;

	/**
	 * Actual speed of the tool given in Cartesian coordinates.
	 * @param value Actual TCP speed (Vector6D object: m/s and rad/s for rotation)
	 */
	actual_TCP_speed: (value: Vector6D) => void;

	/**
	 * Generalized forces in the TCP.
	 * @param value Generalized forces in TCP (Vector6D object)
	 */
	actual_TCP_force: (value: Vector6D) => void;

	/**
	 * Target Cartesian coordinates of the tool.
	 * @param value Target TCP pose (Vector6D object: x, y, z, rx, ry, rz)
	 */
	target_TCP_pose: (value: Vector6D) => void;

	/**
	 * Target speed of the tool given in Cartesian coordinates.
	 * @param value Target TCP speed (Vector6D object: m/s and rad/s for rotation)
	 */
	target_TCP_speed: (value: Vector6D) => void;

	/**
	 * Current state of the digital inputs.
	 * @param value Digital input bits (UINT64)
	 */
	actual_digital_input_bits: (value: number) => void;

	/**
	 * Temperature of each joint in degrees Celsius.
	 * @param value Joint temperatures (Vector6D object)
	 */
	joint_temperatures: (value: Vector6D) => void;

	/**
	 * Controller real-time thread execution time.
	 * @param value Execution time (DOUBLE)
	 */
	actual_execution_time: (value: number) => void;

	/**
	 * Robot mode.
	 * @param value Robot mode (INT32)
	 */
	robot_mode: (value: number) => void;

	/**
	 * Joint control modes.
	 * @param value Joint control modes (Vector6D object)
	 */
	joint_mode: (value: Vector6D) => void;

	/**
	 * Safety mode.
	 * @param value Safety mode (INT32)
	 */
	safety_mode: (value: number) => void;

	/**
	 * Safety status.
	 * @param value Safety status (INT32)
	 */
	safety_status: (value: number) => void;

	/**
	 * Actual tool accelerometer values.
	 * @param value Tool x, y and z accelerometer values (Vector3D object)
	 */
	actual_tool_accelerometer: (value: Vector3D) => void;

	/**
	 * Speed scaling of the trajectory limiter.
	 * @param value Speed scaling (DOUBLE)
	 */
	speed_scaling: (value: number) => void;

	/**
	 * Target speed fraction.
	 * @param value Target speed fraction (DOUBLE)
	 */
	target_speed_fraction: (value: number) => void;

	/**
	 * Norm of Cartesian linear momentum.
	 * @param value Actual momentum (DOUBLE)
	 */
	actual_momentum: (value: number) => void;

	/**
	 * Safety Control Board: Main voltage.
	 * @param value Actual main voltage (DOUBLE)
	 */
	actual_main_voltage: (value: number) => void;

	/**
	 * Safety Control Board: Robot voltage (48V).
	 * @param value Actual robot voltage (DOUBLE)
	 */
	actual_robot_voltage: (value: number) => void;

	/**
	 * Safety Control Board: Robot current.
	 * @param value Actual robot current (DOUBLE)
	 */
	actual_robot_current: (value: number) => void;

	/**
	 * Actual joint voltages.
	 * @param value Actual joint voltages (Vector6D object)
	 */
	actual_joint_voltage: (value: Vector6D) => void;

	/**
	 * Current state of the digital outputs.
	 * @param value Digital output bits (UINT64)
	 */
	actual_digital_output_bits: (value: number) => void;

	/**
	 * Program state.
	 * @param value Program state (UINT32)
	 */
	runtime_state: (value: number) => void;

	/**
	 * Position of robot elbow in Cartesian Base Coordinates.
	 * @param value Elbow position (Vector3D object)
	 */
	elbow_position: (value: Vector3D) => void;

	/**
	 * Velocity of robot elbow in Cartesian Base Coordinates.
	 * @param value Elbow velocity (Vector3D object)
	 */
	elbow_velocity: (value: Vector3D) => void;

	/**
	 * Robot status bits.
	 * @param value Status bits (UINT32)
	 */
	robot_status_bits: (value: number) => void;

	/**
	 * Safety status bits.
	 * @param value Safety status bits (UINT32)
	 */
	safety_status_bits: (value: number) => void;

	/**
	 * Analog I/O types.
	 * @param value Analog I/O types (UINT32)
	 */
	analog_io_types: (value: number) => void;

	/**
	 * Standard analog input 0 [mA or V].
	 * @param value Standard analog input 0 (DOUBLE)
	 */
	standard_analog_input0: (value: number) => void;

	/**
	 * Standard analog input 1 [mA or V].
	 * @param value Standard analog input 1 (DOUBLE)
	 */
	standard_analog_input1: (value: number) => void;

	/**
	 * Standard analog output 0 [mA or V].
	 * @param value Standard analog output 0
	 */
	standard_analog_output0: (value: number) => void;

	/**
	 * Standard analog output 1 [mA or V].
	 * @param value Standard analog output 1
	 */
	standard_analog_output1: (value: number) => void;

	/**
	 * I/O current [mA].
	 * @param value I/O current
	 */
	io_current: (value: number) => void;

	/**
	 * Euromap67 input bits.
	 * @param value Euromap67 input bits
	 */
	euromap67_input_bits: (value: number) => void;

	/**
	 * Euromap67 output bits.
	 * @param value Euromap67 output bits
	 */
	euromap67_output_bits: (value: number) => void;

	/**
	 * Euromap 24V voltage [V].
	 * @param value Euromap 24V voltage
	 */
	euromap67_24V_voltage: (value: number) => void;

	/**
	 * Euromap 24V current [mA].
	 * @param value Euromap 24V current
	 */
	euromap67_24V_current: (value: number) => void;

	/**
	 * Tool mode.
	 * @param value Tool mode
	 */
	tool_mode: (value: number) => void;

	/**
	 * Tool analog input types.
	 * @param value Tool analog input types
	 */
	tool_analog_input_types: (value: number) => void;

	/**
	 * Tool analog input 0 [mA or V].
	 * @param value Tool analog input 0
	 */
	tool_analog_input0: (value: number) => void;

	/**
	 * Tool analog input 1 [mA or V].
	 * @param value Tool analog input 1
	 */
	tool_analog_input1: (value: number) => void;

	/**
	 * Tool output voltage [V].
	 * @param value Tool output voltage
	 */
	tool_output_voltage: (value: number) => void;

	/**
	 * Tool current [mA].
	 * @param value Tool current
	 */
	tool_output_current: (value: number) => void;

	/**
	 * Tool temperature in degrees Celsius.
	 * @param value Tool temperature
	 */
	tool_temperature: (value: number) => void;

	/**
	 * TCP force scalar [N].
	 * @param value TCP force scalar
	 */
	tcp_force_scalar: (value: number) => void;

	/**
	 * General purpose bits (output read back).
	 * @param value General purpose bits
	 */
	output_bit_registers0_to_31: (value: number) => void;

	/**
	 * General purpose bits (output read back).
	 * @param value General purpose bits
	 */
	output_bit_registers32_to_63: (value: number) => void;

	/**
	 * Sets a specific general purpose bit (output).
	 * @param value The boolean value to set
	 * @param index The index of the bit to set (64 general purpose bits index: [64..127])
	 */
	output_bit_register: (value: boolean, index: number) => void;

	/**
	 * Sets a specific general purpose integer (output).
	 * @param value The integer value to set
	 * @param index The index of the integer to set (index range: specify the range here)
	 */
	output_int_register: (value: number, index: number) => void;

	/**
	 * Sets a specific general purpose double (output).
	 * @param value The double value to set
	 * @param index The index of the double to set (index range: specify the range here)
	 */
	output_double_register: (value: number, index: number) => void;

}

export interface ClientToServerEvents {
	/**
 * Determines whether to change the speed slider with this input.
 * @param value 0 = Don't change speed slider with this input, 1 = Use speed_slider_fraction to set speed slider value
 */
	speed_slider_mask: (value: number) => void;

	/**
	 * Sets a new speed slider value.
	 * @param value New speed slider value
	 */
	speed_slider_fraction: (value: number) => void;

	/**
	 * Sets the standard digital output bit mask.
	 * @param value Standard digital output bit mask
	 */
	standard_digital_output_mask: (value: number) => void;

	/**
	 * Sets the configurable digital output bit mask.
	 * @param value Configurable digital output bit mask
	 */
	configurable_digital_output_mask: (value: number) => void;

	/**
	 * Sets the standard digital outputs.
	 * @param value Standard digital outputs
	 */
	standard_digital_output: (value: number) => void;

	/**
	 * Sets the configurable digital outputs.
	 * @param value Configurable digital outputs
	 */
	configurable_digital_output: (value: number) => void;

	/**
	 * Sets the standard analog output mask.
	 * @param value Bits 0-1: standard_analog_output_0 | standard_analog_output_1
	 */
	standard_analog_output_mask: (value: number) => void;

	/**
	 * Sets the output domain for standard analog outputs.
	 * @param value Output domain {0=current[mA], 1=voltage[V]} Bits 0-1: standard_analog_output_0 | standard_analog_output_1
	 */
	standard_analog_output_type: (value: number) => void;

	/**
	 * Sets the standard analog output 0 (ratio) [0..1].
	 * @param value Standard analog output 0 (ratio) [0..1]
	 */
	standard_analog_output_0: (value: number) => void;

	/**
	 * Sets the standard analog output 1 (ratio) [0..1].
	 * @param value Standard analog output 1 (ratio) [0..1]
	 */
	standard_analog_output_1: (value: number) => void;

	/**
	 * Sets general purpose bits.
	 * @param value This range of the boolean input registers is reserved for FieldBus/PLC interface usage.
	 */
	input_bit_registers0_to_31: (value: number) => void;

	/**
	 * Sets general purpose bits.
	 * @param value This range of the boolean input registers is reserved for FieldBus/PLC interface usage.
	 */
	input_bit_registers32_to_63: (value: number) => void;

	/**
 * Sets a specific general purpose bit.
 * @param value The boolean value to set
 * @param index The index of the bit to set (64 general purpose bits index: [64..127])
 */
	input_bit_register: (value: boolean, index: number) => void;

	/**
	 * Sets a specific general purpose integer.
	 * @param value The integer value to set
	 * @param index The index of the integer to set (index range: specify the range here)
	 */
	input_int_register: (value: number, index: number) => void;

	/**
	 * Sets a specific general purpose double.
	 * @param value The double value to set
	 * @param index The index of the double to set (index range: specify the range here)
	 */
	input_double_register: (value: number, index: number) => void;

	/**
 * Sets the external force-torque values.
 * This command allows for setting the external force and torque applied to the robot.
 * @param value A Vector6D object representing the forces and torques.
 */
	external_force_torque: (value: Vector6D) => void;

	/**
 * The current output mode.
 * @param value The current output mode (UINT8)
 */
	tool_output_mode: (value: number) => void;

	/**
	 * The current mode of digital output 0.
	 * @param value The current mode of digital output 0 (UINT8)
	 */
	tool_digital_output0_mode: (value: number) => void;

	/**
	 * The current mode of digital output 1.
	 * @param value The current mode of digital output 1 (UINT8)
	 */
	tool_digital_output1_mode: (value: number) => void;

	/**
	 * Payload mass in Kg.
	 * @param value Payload mass (DOUBLE)
	 */
	payload: (value: number) => void;

	/**
	 * Payload Center of Gravity (CoG) in meters (CoGx, CoGy, CoGz).
	 * @param value Payload CoG (Vector3D object)
	 */
	payload_cog: (value: Vector3D) => void;

	/**
	 * Payload inertia matrix elements [Ixx, Iyy, Izz, Ixy, Ixz, Iyz] expressed in kg*m^2.
	 * @param value Payload inertia matrix elements (Vector6D object)
	 */
	payload_inertia: (value: Vector6D) => void;

	/**
	 * Script line number that is actually in control of the robot, given the robot is locked by one of the threads in the script.
	 * If no thread is locking the robot, this field is set to '0'.
	 * Script line number should not be confused with program tree line number displayed on PolyScope.
	 * @param value Script control line number (UINT32)
	 */
	script_control_line: (value: number) => void;

	/**
	 * Raw force and torque measurement given in the Tool Flange frame.
	 * Not compensated for forces and torques caused by the payload. Not zeroed by zero_ftsensor().
	 * @param value Raw force and torque measurement (Vector6D object)
	 */
	ft_raw_wrench: (value: Vector6D) => void;

	/**
	 * A non-negative double precision floating point value which can be used to monitor how close the robot is to triggering a C153 or C159 "joint position deviation" protective stop.
	 * A protective stop is triggered if the value is ever equal to or exceeds 1.0.
	 * @param value Joint position deviation ratio (DOUBLE)
	 */
	joint_position_deviation_ratio: (value: number) => void;
}

export interface InterServerEvents {
	ping: () => void
}

export interface SocketData {
	id: string
	target: {
		name: string
		slug: string
		address: string
		type: 'real' | 'virtual'
	}
}

// create the socket type
export type Socket = BareSocket<ClientToServerEvents, ServerToClientEvents, InterServerEvents, SocketData>