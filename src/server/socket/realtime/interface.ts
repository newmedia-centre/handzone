/**
 * Struct containing all of the data send using the realtime 5.10 protocol
 * https://s3-eu-west-1.amazonaws.com/ur-support-site/16496/ClientInterfaces_Realtime.pdf
 */
export type RealtimeData = {
	/** Total message length in bytes */
	message_size: number
	/** Time elapsed since the controller was started */
	time: number
	/** Target joint positions */
	q_target: number[]
	/** Target joint velocities */
	qd_target: number[]
	/** Target joint accelerations */
	qdd_target: number[]
	/** Target joint currents */
	i_target: number[]
	/** Target joint moments (torques) */
	m_target: number[]
	/** Actual joint positions */
	q_actual: number[]
	/** Actual joint velocities */
	qd_actual: number[]
	/** Actual joint currents */
	i_actual: number[]
	/** Joint control currents */
	i_control: number[]
	/** Actual Cartesian coordinates of the tool: (x,y,z,rx,ry,rz) */
	tool_vector_actual: number[]
	/** Actual speed of the tool given in Cartesian coordinates */
	tcp_speed_actual: number[]
	/** Generalised forces in the TCP */
	tcp_force: number[]
	/** Target Cartesian coordinates of the tool: (x,y,z,rx,ry,rz) */
	tool_vector_target: number[]
	/** Target speed of the tool given in Cartesian coordinates */
	tcp_speed_target: number[]
	/** Current state of the digital inputs. NOTE: these are bits encoded as int64_t, e.g. a value of 5 corresponds to bit 0 and bit 2 set high */
	digital_input_bits: number
	/** Temperature of each joint in degrees celsius */
	motor_temperatures: number[]
	/** Controller realtime thread execution time */
	controller_timer: number
	/** Robot mode */
	robot_mode: number
	/** Joint control modes */
	joint_modes: number[]
	/** Safety mode */
	safety_mode: number
	/** Tool x,y and z accelerometer values (software version 1.7) */
	tool_accelerometer_values: number[]
	/** Speed scaling of the trajectory limiter */
	speed_scaling: number
	/** Norm of Cartesian linear momentum */
	linear_momentum_norm: number
	/** Masterboard: Main voltage */
	v_main: number
	/** Masterboard: Robot voltage (48V) */
	v_robot: number
	/** Masterboard: Robot current */
	i_robot: number
	/** Actual joint voltages */
	v_actual: number[]
	/** Digital outputs */
	digital_outputs: number
	/** Program state */
	program_state: number
	/** Elbow position */
	elbow_position: number[]
	/** Elbow velocity */
	elbow_velocity: number[]
	/** Safety status */
	safety_status: number
	/** Payload Mass [kg] */
	payload_mass: number
	/** Payload Center of Gravity (x, y, z) [m] */
	payload_cog: number[]
	/** Payload Inertia (Ixx, Iyy, Izz, Ixy, Ixz, Iyz) [kg*m^2] */
	payload_inertia: number[]
}

// declare socket.io interfaces
export interface RealtimeServerToClient {
	/**
	 * The raw data from the robot, encoded as a base64 string.
	 */
	'realtime:raw': (data: string) => void

	/** The parsed data from the robot */
	'realtime:data': (data: RealtimeData) => void
}