import type { Vector6D } from '../interface'

export interface MotionClientToServer {
	/**
	* Deprecated: Tells the robot controller to treat digital inputs number A and B as pulses for a conveyor encoder.
	* Only digital input 0, 1, 2, or 3 can be used. This function is replaced by encoder_enable_pulse_decode and
	* should therefore not be used moving forward.
	*
	* @param type An integer determining how to treat the inputs on A and B:
	*             0 - No encoder, pulse decoding is disabled.
	*             1 - Quadrature encoder, input A and B must be square waves with 90 degree offset. Direction of the conveyor can be determined.
	*             2 - Rising and falling edge on single input (A).
	*             3 - Rising edge on single input (A).
	*             4 - Falling edge on single input (A).
	*             The controller can decode inputs at up to 40kHz.
	* @param A Encoder input A, values of 0-3 are the digital inputs 0-3.
	* @param B Encoder input B, values of 0-3 are the digital inputs 0-3.
	*/
	'motion:conveyor_pulse_decode': (type: number, A: number, B: number) => void

	/**
	* Sets up an encoder hooked up to the pulse decoder of the controller.
	*
	* @param encoder_index Index of the encoder to define. Must be either 0 or 1.
	* @param decoder_type An integer determining how to treat the inputs on A and B:
	*                     0 - No encoder, pulse decoding is disabled.
	*                     1 - Quadrature encoder, input A and B must be square waves with 90 degree offset. Direction of the conveyor can be determined.
	*                     2 - Rising and falling edge on single input (A).
	*                     3 - Rising edge on single input (A).
	*                     4 - Falling edge on single input (A).
	*                     The controller can decode inputs at up to 40kHz.
	* @param A Encoder input A pin. Must be 0-3.
	* @param B Encoder input B pin. Must be 0-3.
	*/
	'motion:encoder_enable_pulse_decode': (encoder_index: number, decoder_type: number, A: number, B: number) => void

	/**
	* Sets up an encoder expecting to be updated with tick counts via the function encoder_set_tick_count.
	*
	* @param encoder_index Index of the encoder to define. Must be either 0 or 1.
	* @param range_id Range of the encoder (integer). Needed to handle wrapping nicely.
	*                 0 - 32 bit signed encoder, range [-2147483648  2147483647].
	*                 1 - 8 bit unsigned encoder, range [0  255].
	*                 2 - 16 bit unsigned encoder, range [0  65535].
	*                 3 - 24 bit unsigned encoder, range [0  16777215].
	*                 4 - 32 bit unsigned encoder, range [0  4294967295].
	*/
	'motion:encoder_enable_set_tick_count': (encoder_index: number, range_id: number) => void

	/**
	* Returns the tick count of the designated encoder.
	*
	* @param encoder_index Index of the encoder to query. Must be either 0 or 1.
	* @returns The conveyor encoder tick count (float).
	*/
	'motion:encoder_get_tick_count': (encoder_index: number) => number

	/**
	 * Sets the active tcp offset, i.e. the transformation from the output flange coordinate system to the TCP as a pose.
	 * 
	 * @param pose The TCP pose to set.
	 */
	'motion:set_tcp': (pose: number[]) => void
	
	/**
	* Tells the robot controller the tick count of the encoder. This function is useful for absolute encoders (e.g., MODBUS).
	* Assumes that the encoder is enabled using encoder_enable_set_tick_count first.
	*
	* @param encoder_index Index of the encoder to define. Must be either 0 or 1.
	* @param count The tick count to set. Must be within the range of the encoder.
	*/
	'motion:encoder_set_tick_count': (encoder_index: number, count: number) => void

	/**
	* Returns the delta_tick_count. Unwinds in case encoder wraps around the range. If no wrapping has happened, the given delta_tick_count is returned without any modification.
	* This heuristic function checks that a given delta_tick_count value is reasonable. If the encoder wrapped around the end of the range, it compensates (i.e., unwinds) and returns the adjusted result. If a delta_tick_count is larger than half the range of the encoder, wrapping is assumed and is compensated. As a consequence, this function only works when the range of the encoder is explicitly known, and therefore the designated encoder must be enabled. If not, this function will always return nil.
	*
	* @param encoder_index Index of the encoder to query. Must be either 0 or 1.
	* @param delta_tick_count The delta (difference between two) tick count to unwind (float).
	* @returns The unwound delta_tick_count (float).
	*/
	'motion:encoder_unwind_delta_tick_count': (encoder_index: number, delta_tick_count: number) => number

	/**
	* Resets the robot mode from force mode to normal operation. This is also done when a program stops.
	*/
	'motion:end_force_mode': () => void

	/**
	* Set robot back in normal position control mode after freedrive mode.
	*/
	'motion:end_freedrive_mode': () => void

	/**
	* Set robot back in normal position control mode after teach mode.
	*/
	'motion:end_teach_mode': () => void

	/**
	* Set robot to be controlled in force mode.
	*
	* @param task_frame A pose vector that defines the force frame relative to the base frame.
	* @param selection_vector A 6d vector of 0s and 1s. 1 means that the robot will be compliant in the corresponding axis of the task frame.
	* @param wrench The forces/torques the robot will apply to its environment. The robot adjusts its position along/about compliant axis in order to achieve the specified force/torque. Values have no effect for non-compliant axes. Actual wrench applied may be lower than requested due to joint safety limits. Actual forces and torques can be read using get_tcp_force function in a separate thread.
	* @param type An integer [13] specifying how the robot interprets the force frame:
	*             1 - The force frame is transformed in a way such that its y-axis is aligned with a vector pointing from the robot tcp towards the origin of the force frame.
	*             2 - The force frame is not transformed.
	*             3 - The force frame is transformed in a way such that its x-axis is the projection of the robot tcp velocity vector onto the x-y plane of the force frame.
	* @param limits (Float) 6d vector. For compliant axes, these values are the maximum allowed tcp speed along/about the axis. For non-compliant axes, these values are the maximum allowed deviation along/about an axis between the actual tcp position and the one set by the program.
	*/
	'motion:force_mode': (task_frame: number[], selection_vector: number[], wrench: number[], type: number, limits: number[]) => void

	/**
	* Sets the damping parameter in force mode.
	*
	* @param damping Between 0 and 1, default value is 0. A value of 1 is full damping, so the robot will decelerate quickly if no force is present. A value of 0 is no damping, here the robot will maintain the speed. The value is stored until this function is called again. Add this to the beginning of your program to ensure it is called before force mode is entered (otherwise default value will be used).
	*/
	'motion:force_mode_set_damping': (damping: number) => void

	/**
	* Set robot in freedrive mode. In this mode the robot can be moved around by hand in the same way as by pressing the "freedrive" button. The robot will not be able to follow a trajectory (e.g., a movej) in this mode.
	*/
	'motion:freedrive_mode': () => void

	/**
	* Set robot in freedrive mode similar to using freedrive_mode(). The function takes no parameters, but enters freedrive mode with movement possible in all directions and around all rotational axes.
	* Note: When entering freedrive mode, a check detecting incorrectly specified payload will not be performed. This specifically allows the user to push the robot when entering freedrive mode without causing a protective stop, but also means that equipment attached to the robot arm may cause the robot to start moving unexpectedly unless the correct payload mass and center of gravity offset are specified in the installation.
	*/
	'motion:freedrive_mode_no_incorrect_payload_check': () => void

	/**
	* Deprecated: Tells the tick count of the encoder. Note that the controller interpolates tick counts to get more accurate movements with low resolution encoders.
	* Deprecated: This function is replaced by encoder_get_tick_count and it should therefore not be used moving forward.
	*
	* @returns The conveyor encoder tick count.
	*/
	'motion:get_conveyor_tick_count': () => number

	/**
	* Query the target TCP pose as given by the trajectory being followed.
	* This script function is useful in conjunction with conveyor tracking to know what the target pose of the TCP would be if no offset was applied.
	*
	* @returns Target TCP pose.
	*/
	'motion:get_target_tcp_pose_along_path': () => number[]

	/**
	* Query the target TCP speed as given by the trajectory being followed.
	* This script function is useful in conjunction with conveyor tracking to know what the target speed of the TCP would be if no offset was applied.
	*
	* @returns Target TCP speed as a vector.
	*/
	'motion:get_target_tcp_speed_along_path': () => number[]

	/**
	* Move Circular: Move to position (circular in tool-space).
	* TCP moves on the circular arc segment from current pose, through pose_via to pose_to. Accelerates to and moves with constant tool speed v. Use the mode parameter to define the orientation interpolation.
	*
	* @param pose_via Path point (note: only position is used). Pose_via can also be specified as joint positions, then forward kinematics is used to calculate the corresponding pose.
	* @param pose_to Target pose (note: only position is used in Fixed orientation mode). Pose_to can also be specified as joint positions, then forward kinematics is used to calculate the corresponding pose.
	* @param a Tool acceleration [m/s^2] (default: 1.2).
	* @param v Tool speed [m/s] (default: 0.25).
	* @param r Blend radius (of target pose) [m] (default: 0).
	* @param mode 0: Unconstrained mode. Interpolate orientation from current pose to target pose (pose_to).
	*             1: Fixed mode. Keep orientation constant relative to the tangent of the circular arc (starting from current pose).
	*/
	'motion:movec': (pose_via: number[], pose_to: number[], a: number, v: number, r: number, mode: number) => void

	/**
	* Move to position (linear in joint-space).
	* When using this command, the robot must be at a standstill or come from a movej or movel with a blend. The speed and acceleration parameters control the trapezoid speed profile of the move. Alternatively, the t parameter can be used to set the time for this move. Time setting has priority over speed and acceleration settings.
	*
	* @param q Joint positions (q can also be specified as a pose, then inverse kinematics is used to calculate the corresponding joint positions).
	* @param a Joint acceleration of leading axis [rad/s^2] (default: 1.4).
	* @param v Joint speed of leading axis [rad/s] (default: 1.05).
	* @param t Time [s] (default: 0).
	* @param r Blend radius [m] (default: 0).
	* If a blend radius is set, the robot arm trajectory will be modified to avoid the robot stopping at the point. However, if the blend region of this move overlaps with the blend radius of previous or following waypoints, this move will be skipped, and an 'Overlapping Blends' warning message will be generated.
	*/
	'motion:movej': (q: number[], a: number, v: number, t: number, r: number) => void

	/**
	* Move to position (linear in tool-space).
	*
	* @param pose Target pose (pose can also be specified as joint positions, then forward kinematics is used to calculate the corresponding pose).
	* @param a Tool acceleration [m/s^2] (default: 1.2).
	* @param v Tool speed [m/s] (default: 0.25).
	* @param t Time [s] (default: 0).
	* @param r Blend radius [m] (default: 0).
	*/
	'motion:movel': (pose: number[], a: number, v: number, t: number, r: number) => void

	/**
	* Move Process: Blend circular (in tool-space) and move linear (in tool-space) to position. Accelerates to and moves with constant tool speed v.
	*
	* @param pose Target pose (pose can also be specified as joint positions, then forward kinematics is used to calculate the corresponding pose).
	* @param a Tool acceleration [m/s^2] (default: 1.2).
	* @param v Tool speed [m/s] (default: 0.25).
	* @param r Blend radius [m] (default: 0).
	*/
	'motion:movep': (pose: number[], a: number, v: number, r: number) => void

	/**
	* Makes the robot pause if the specified error code occurs. The robot will only pause during program execution. This setting is reset when the program is stopped - call the command again before/during program execution to re-enable it.
	*
	* @param code The code of the error for which the robot should pause (int).
	* @param argument The argument of the error. If this parameter is omitted, the robot will pause on any argument for the specified error code (int). Optional.
	*
	* Notes:
	* - Error codes appear in the log as CxAy where 'x' is the code and 'y' is the argument.
	* - There is a short delay from when the pause is triggered until the robot is fully paused. In some special cases, the program might reach the subsequent program line before it is paused.
	*/
	'motion:pause_on_error_code': (code: number, argument?: number) => void

	/**
	* When enabled, this function generates warning messages to the log when the robot deviates from the target position. This function can be called at any point in the execution of a program. It has no return value.
	*
	* @param enabled (Boolean) Enable or disable position deviation log messages.
	* @param threshold (Float) Optional value in the range [01], where 0 is no position deviation and 1 is the maximum position deviation (equivalent to the amount of position deviation that causes a protective stop of the robot). If no threshold is specified by the user, a default value of 0.8 is used.
	*/
	'motion:position_deviation_warning': (enabled: boolean, threshold?: number) => void

	/**
	* Reset the revolution counter, if no offset is specified. This is applied on joints which safety limits are set to "Unlimited" and are only applied when new safety settings are applied with limited joint angles.
	*
	* @param qNear Optional parameter, reset the revolution counter to one close to the given qNear joint vector. If not defined, the joint’s actual number of revolutions are used.
	*/
	'motion:reset_revolution_counter': (qNear?: number[]) => void

	/**
	* Servoj can be used for online realtime control of joint positions.
	* The gain parameter works the same way as the P-term of a PID controller, where it adjusts the current position towards the desired (q). The higher the gain, the faster reaction the robot will have.
	* The parameter lookahead_time is used to project the current position forward in time with the current velocity. A low value gives fast reaction, a high value prevents overshoot.
	* Note: A high gain or a short lookahead time may cause instability and vibrations. Especially if the target positions are noisy or updated at a low frequency.
	* It is preferred to call this function with a new setpoint (q) in each time step (thus the default t=0.008).
	*
	* @param q Joint angles in radians representing rotations of base, shoulder, elbow, wrist1, wrist2, and wrist3.
	* @param a Not used in current version (reserved for future use).
	* @param v Not used in current version (reserved for future use).
	* @param t Time where the command is controlling the robot. The function is blocking for time t [s] (default: 0.008).
	* @param lookahead_time Time [s], range [0.03,0.2] smoothens the trajectory with this lookahead time (default: 0.1).
	* @param gain Proportional gain for following target position, range [100,2000] (default: 300).
	*/
	'motion:servoj': (q: number[], a: number, v: number, t?: number, lookahead_time?: number, gain?: number) => void

	/**
	* Deprecated: Tells the robot controller the tick count of the encoder. This function is useful for absolute encoders, use conveyor_pulse_decode() for setting up an incremental encoder. For circular conveyors, the value must be between 0 and the number of ticks per revolution.
	*
	* @param tick_count Tick count of the conveyor (Integer).
	* @param absolute_encoder_resolution Resolution of the encoder, needed to handle wrapping nicely (Integer).
	*        0 is a 32 bit signed encoder, range [-2147483648  2147483647] (default).
	*        1 is an 8 bit unsigned encoder, range [0  255].
	*        2 is a 16 bit unsigned encoder, range [0  65535].
	*        3 is a 24 bit unsigned encoder, range [0  16777215].
	*        4 is a 32 bit unsigned encoder, range [0  4294967295].
	*
	* Deprecated: This function is replaced by encoder_set_tick_count and it should therefore not be used moving forward.
	*/
	'motion:set_conveyor_tick_count': (tick_count: number, absolute_encoder_resolution?: number) => void

	/**
	* Set joint positions of simulated robot.
	*
	* @param q Joint positions.
	*
	* Example command: set_pos([0.0, 1.57, -1.57, 0, 0, 3.14])
	* - Example Parameters:
	*   - q = [0.0, 1.57, -1.57, 0, 0, 3.14] → the position of the simulated robot with joint angles in radians representing rotations of base, shoulder, elbow, wrist1, wrist2, and wrist3.
	*/
	'motion:set_pos': (q: number[]) => void

	/**
	* Sets the transition hardness between normal mode, reduced mode, and safeguard stop.
	*
	* @param type An integer specifying transition hardness.
	* - 0 is hard transition between modes using maximum torque, similar to emergency stop.
	* - 1 is soft transition between modes.
	*/
	'motion:set_safety_mode_transition_hardness': (type: number) => void

	/**
	* Joint speed: Accelerate linearly in joint space and continue with constant joint speed.
	* The time t is optional if provided the function will return after time t, regardless of the target speed has been reached.
	* If the time t is not provided, the function will return when the target speed is reached.
	*
	* @param qd Joint speeds [rad/s].
	* @param a Joint acceleration [rad/s^2] (of leading axis).
	* @param t Time [s] before the function returns (optional).
	*
	* Example command: speedj([0.2, 0.3, 0.1, 0.05, 0, 0], 0.5, 0.5)
	* - Example Parameters:
	*   - qd → Joint speeds of: base=0.2 rad/s, shoulder=0.3 rad/s, elbow=0.1 rad/s, wrist1=0.05 rad/s, wrist2 and wrist3=0 rad/s.
	*   - a = 0.5 rad/s^2 → acceleration of the leading axis (shoulder in this case).
	*   - t = 0.5 s → time before the function returns.
	*/
	'motion:speedj': (qd: number[], a: number, t?: number) => void

	/**
	* Tool speed: Accelerate linearly in Cartesian space and continue with constant tool speed.
	* The time t is optional if provided the function will return after time t, regardless of the target speed has been reached.
	* If the time t is not provided, the function will return when the target speed is reached.
	*
	* @param xd Tool speed [m/s] (spatial vector).
	* @param a Tool position acceleration [m/s^2].
	* @param t Time [s] before function returns (optional).
	* @param aRot Tool acceleration [rad/s^2] (optional), if not defined a, position acceleration, is used.
	*
	* Example command: speedl([0.5, 0.4, 0, 1.57, 0, 0], 0.5, 0.5)
	* - Example Parameters:
	*   - xd → Tool speeds of: x=500 mm/s, y=400 mm/s, rx=90 deg/s, ry and rz=0 mm/s.
	*   - a = 0.5 rad/s^2 → acceleration of the leading axis (shoulder in this case).
	*   - t = 0.5 s → time before the function returns.
	*/
	'motion:speedl': (xd: number[], a: number, t?: number, aRot?: string) => void
	
	/**
	 * Calculate the forward kinematics for the robot. This function returns the pose of the robot's tool center point (TCP) given the joint angles and the tool center point (TCP) offset.
	 *
	 * @param q Joint angles in radians representing rotations of base, shoulder, elbow, wrist1, wrist2, and wrist3.
	 * @param tcp_offset The tool center point (TCP) offset pose vector.
	 *
	 * Example command: get_forward_kin([0.0, 1.57, -1.57, 0, 0, 3.14], [0.1, 0.2, 0.3, 0, 0, 0])
	 * - Example Parameters:
	 *  - q = [0.0, 1.57, -1.57, 0, 0, 3.14] → the joint angles in radians representing rotations of base, shoulder, elbow, wrist1, wrist2, and wrist3.
	 *  - tcp = [0.1, 0.2, 0.3, 0, 0, 0] → the tool center point (TCP) offset pose vector.
	 */
	'motion:get_forward_kin': (tcp_offset?: number[]) => void
	
	/**
	* Stop tracking the conveyor, started by track_conveyor_linear() or track_conveyor_circular(),
	* and decelerate all joint speeds to zero.
	*
	* @param a Joint acceleration [rad/s^2] (optional).
	*
	* Example command: stop_conveyor_tracking(a=15)
	* - Example Parameters:
	*   - a = 15 rad/s^2 → acceleration of the joints.
	*/
	'motion:stop_conveyor_tracking': (a?: number) => void

	/**
	* Stop (linear in joint space): Decelerate joint speeds to zero.
	*
	* @param a Joint acceleration [rad/s^2] (of leading axis).
	*
	* Example command: stopj(2)
	* - Example Parameters:
	*   - a = 2 rad/s^2 → rate of deceleration of the leading axis.
	*/
	'motion:stopj': (a: number) => void

	/**
	* Stop (linear in tool space): Decelerate tool speed to zero.
	*
	* @param a Tool acceleration [m/s^2].
	* @param aRot Tool acceleration [rad/s^2] (optional), if not defined a, position acceleration, is used.
	*
	* Example command: stopl(20)
	* - Example Parameters:
	*   - a = 20 m/s^2 → rate of deceleration of the tool.
	*   - aRot → tool deceleration [rad/s^2] (optional), if not defined, position acceleration, is used.
	*/
	'motion:stopl': (a: number, aRot?: string) => void

	/**
	* Set robot in freedrive mode. In this mode the robot can be moved around by hand in the same way as by pressing the "freedrive" button.
	* The robot will not be able to follow a trajectory (eg. a movej) in this mode.
	*/
	'motion:teach_mode': () => void

	/**
	* Makes robot movement (movej() etc.) track a circular conveyor.
	*
	* @param center Pose vector that determines the center of the conveyor in the base coordinate system of the robot.
	* @param ticksPerRevolution How many ticks the encoder sees when the conveyor moves one revolution.
	* @param rotateTool Should the tool rotate with the conveyor or stay in the orientation specified by the trajectory (movel() etc.).
	* @param encoderIndex The index of the encoder to associate with the conveyor tracking (optional, default is 0).
	*
	* Example command: track_conveyor_circular(p[0.5,0.5,0,0,0,0], 500.0, false)
	* - Example Parameters:
	*   - center = p[0.5,0.5,0,0,0,0] → location of the center of the conveyor.
	*   - ticksPerRevolution = 500.0 → the number of ticks the encoder sees when the conveyor moves one revolution.
	*   - rotateTool = false → the tool should not rotate with the conveyor, but stay in the orientation specified by the trajectory (movel() etc.).
	*   - encoderIndex = 0 → the index of the encoder to associate with the conveyor tracking (optional, default is 0).
	*/
	'motion:track_conveyor_circular': (center: Vector6D, ticksPerRevolution: number, rotateTool: boolean, encoderIndex?: number) => void

	/**
	* Makes robot movement (movej() etc.) track a linear conveyor.
	*
	* @param direction Pose vector that determines the direction of the conveyor in the base coordinate system of the robot.
	* @param ticksPerMeter How many ticks the encoder sees when the conveyor moves one meter.
	* @param encoderIndex The index of the encoder to associate with the conveyor tracking (optional, default is 0).
	*
	* Example command: track_conveyor_linear(p[1,0,0,0,0,0], 1000.0)
	* - Example Parameters:
	*   - direction = p[1,0,0,0,0,0] → Pose vector that determines the direction of the conveyor in the base coordinate system of the robot.
	*   - ticksPerMeter = 1000.0 → How many ticks the encoder sees when the conveyor moves one meter.
	*   - encoderIndex = 0 → The index of the encoder to associate with the conveyor tracking (optional, default is 0).
	*/
	'motion:track_conveyor_linear': (direction: Vector6D, ticksPerMeter: number, encoderIndex?: number) => void
}