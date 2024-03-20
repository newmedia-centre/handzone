/**
	* Deprecated: Tells the robot controller to treat digital inputs number A and B as pulses for a conveyor encoder.
	* Only digital input 0, 1, 2, or 3 can be used. This function is replaced by encoder_enable_pulse_decode and
	* should therefore not be used moving forward. */
export type MotionConveyorPulseDecodeIn = {
	/** An integer determining how to treat the inputs on A and B:
	 * 0 - No encoder, pulse decoding is disabled.
	 * 1 - Quadrature encoder, input A and B must be square waves with 90 degree offset. Direction of the conveyor can be determined.
	 * 2 - Rising and falling edge on single input (A).
	 * 3 - Rising edge on single input (A).
	 * 4 - Falling edge on single input (A).
	 * The controller can decode inputs at up to 40kHz. */
	type: number
	/** Encoder input A, values of 0-3 are the digital inputs 0-3. */
	A: number
	/** Encoder input B, values of 0-3 are the digital inputs 0-3. */
	B: number
}

/** Sets up an encoder hooked up to the pulse decoder of the controller. */
export type MotionEncoderEnablePulseDecodeIn = {
	/** Index of the encoder to define. Must be either 0 or 1. */
	encoder_index: number
	/** An integer determining how to treat the inputs on A and B:
	 * 0 - No encoder, pulse decoding is disabled.
	 * 1 - Quadrature encoder, input A and B must be square waves with 90 degree offset. Direction of the conveyor can be determined.
	 * 2 - Rising and falling edge on single input (A).
	 * 3 - Rising edge on single input (A).
	 * 4 - Falling edge on single input (A).
	 * The controller can decode inputs at up to 40kHz. */
	decoder_type: number
	/** Encoder input A, values of 0-3 are the digital inputs 0-3. */
	A: number
	/** Encoder input B, values of 0-3 are the digital inputs 0-3. */
	B: number
}

/** Sets up an encoder expecting to be updated with tick counts via the function encoder_set_tick_count. */
export type MotionEncoderEnableSetTickCountIn = {
	/** Index of the encoder to define. Must be either 0 or 1. */
	encoder_index: number
	/** Range of the encoder (integer). Needed to handle wrapping nicely.
	 * 0 - 32 bit signed encoder, range [-2147483648  2147483647].
	 * 1 - 8 bit unsigned encoder, range [0  255].
	 * 2 - 16 bit unsigned encoder, range [0  65535].
	 * 3 - 24 bit unsigned encoder, range [0  16777215].
	 * 4 - 32 bit unsigned encoder, range [0  4294967295]. */
	range_id: number
}

/** Returns the tick count of the designated encoder. */
export type MotionEncoderGetTickCountIn = {
	/** Index of the encoder to query. Must be either 0 or 1. */
	encoder_index: number
}

export type MotionEncoderGetTickCountCallback = {
	/** The conveyor encoder tick count (float). */
	count: number
}

/** Sets the active tcp offset, i.e. the transformation from the output flange coordinate system to the TCP as a pose. */
export type MotionSetTCPIn = {
	/** The TCP pose to set. */
	pose: number[]
}

/**
	* Tells the robot controller the tick count of the encoder. This function is useful for absolute encoders (e.g., MODBUS).
	* Assumes that the encoder is enabled using encoder_enable_set_tick_count first. */
export type MotionEncoderSetTickCountIn = {
	/** Index of the encoder to define. Must be either 0 or 1. */
	encoder_index: number
	/** The tick count to set. Must be within the range of the encoder. */
	count: number
}

/**
	* Returns the delta_tick_count. Unwinds in case encoder wraps around the range. If no wrapping has happened, the given delta_tick_count is returned without any modification.
	* This heuristic function checks that a given delta_tick_count value is reasonable. If the encoder wrapped around the end of the range, it compensates (i.e., unwinds) and returns the adjusted result. If a delta_tick_count is larger than half the range of the encoder, wrapping is assumed and is compensated. As a consequence, this function only works when the range of the encoder is explicitly known, and therefore the designated encoder must be enabled. If not, this function will always return nil. */
export type MotionEncoderUnwindDeltaTickCountIn = {
	/** Index of the encoder to query. Must be either 0 or 1. */
	encoder_index: number
	/** The delta (difference between two) tick count to unwind (float). */
	delta_tick_count: number
}

/**
 * Set robot to be controlled in force mode.
 */
export type MotionForceModeIn = {
	/** A pose vector that defines the force frame relative to the base frame. */
	task_frame: number[]
	/** A 6d vector of 0s and 1s. 1 means that the robot will be compliant in the corresponding axis of the task frame. */
	selection_vector: number[]
	/** The forces/torques the robot will apply to its environment. */
	wrench: number[]
	/** An integer [13] specifying how the robot interprets the force frame. */
	type: number
	/** (Float) 6d vector. For compliant axes, these values are the maximum allowed tcp speed along/about the axis. */
	limits: number[]
}

/**
 * Sets the damping parameter in force mode.
 */
export type MotionForceModeSetDampingIn = {
	/** Between 0 and 1, default value is 0. */
	damping: number
}

/**
	* Deprecated: Tells the tick count of the encoder. Note that the controller interpolates tick counts to get more accurate movements with low resolution encoders.
	* Deprecated: This function is replaced by encoder_get_tick_count and it should therefore not be used moving forward. */
export type MotionGetConveyorTickCountCallback = {
	/** The conveyor encoder tick count. */
	count: number
}

/**
	* Query the target TCP pose as given by the trajectory being followed.
	* This script function is useful in conjunction with conveyor tracking to know what the target pose of the TCP would be if no offset was applied. */
export type MotionGetTargetTCPPoseAlongPathCallback = {
	/** Target TCP pose. */
	pose: number[]
}

/**
	* Query the target TCP speed as given by the trajectory being followed.
	* This script function is useful in conjunction with conveyor tracking to know what the target speed of the TCP would be if no offset was applied. */
export type MotionGetTargetTCPSpeedAlongPathCallback = {
	/** Target TCP speed as a vector. */
	speed: number[]
}

/**
	* Move Circular: Move to position (circular in tool-space).
	* TCP moves on the circular arc segment from current pose, through pose_via to pose_to. Accelerates to and moves with constant tool speed v. Use the mode parameter to define the orientation interpolation. */
export type MotionMoveCIn = {
	/** Path point (note: only position is used). Pose_via can also be specified as joint positions, then forward kinematics is used to calculate the corresponding pose. */
	pose_via: number[]
	/** Target pose (note: only position is used in Fixed orientation mode). Pose_to can also be specified as joint positions, then forward kinematics is used to calculate the corresponding pose. */
	pose_to: number[]
	/** Tool acceleration [m/s^2] (default: 1.2). */
	a: number
	/** Tool speed [m/s] (default: 0.25). */
	v: number
	/** Blend radius (of target pose) [m] (default: 0). */
	r: number
	/** 0: Unconstrained mode. Interpolate orientation from current pose to target pose (pose_to).
	 * 1: Fixed mode. Keep orientation constant relative to the tangent of the circular arc (starting from current pose). */
	mode: number
}

/**
	* Move to position (linear in joint-space).
	* When using this command, the robot must be at a standstill or come from a movej or movel with a blend. The speed and acceleration parameters control the trapezoid speed profile of the move. Alternatively, the t parameter can be used to set the time for this move. Time setting has priority over speed and acceleration settings. */
export type MotionMoveJIn = {
	/** Joint positions (q can also be specified as a pose, then inverse kinematics is used to calculate the corresponding joint positions). */
	q: number[]
	/** Joint acceleration of leading axis [rad/s^2] (default: 1.4). */
	a: number
	/** Joint speed of leading axis [rad/s] (default: 1.05). */
	v: number
	/** Time [s] (default: 0). */
	t: number
	/** Blend radius [m] (default: 0). If a blend radius is set, the robot arm trajectory will be modified to avoid the robot stopping at the point. However, if the blend region of this move overlaps with the blend radius of previous or following waypoints, this move will be skipped, and an 'Overlapping Blends' warning message will be generated. */
	r: number
}

/** Move to position (linear in tool-space). */
export type MotionMoveLIn = {
	/** Target pose (pose can also be specified as joint positions, then forward kinematics is used to calculate the corresponding pose). */
	pose: number[]
	/** Tool acceleration [m/s^2] (default: 1.2). */
	a: number
	/** Tool speed [m/s] (default: 0.25). */
	v: number
	/** Time [s] (default: 0). */
	t: number
	/** Blend radius [m] (default: 0). */
	r: number
}

/** Move Process: Blend circular (in tool-space) and move linear (in tool-space) to position. Accelerates to and moves with constant tool speed v. */
export type MotionMovePIn = {
	/** Target pose (pose can also be specified as joint positions, then forward kinematics is used to calculate the corresponding pose). */
	pose: number[]
	/** Tool acceleration [m/s^2] (default: 1.2). */
	a: number
	/** Tool speed [m/s] (default: 0.25). */
	v: number
	/** Blend radius [m] (default: 0). */
	r: number
}

/** Makes the robot pause if the specified error code occurs. The robot will only pause during program execution. This setting is reset when the program is stopped - call the command again before/during program execution to re-enable it. */
export type MotionPauseOnErrorCodeIn = {
	/** The code of the error for which the robot should pause. */
	code: number
	/** The argument of the error. If this parameter is omitted, the robot will pause on any argument for the specified error code. */
	argument?: number
}

/** When enabled, this function generates warning messages to the log when the robot deviates from the target position. This function can be called at any point in the execution of a program. It has no return value. */
export type MotionPositionDeviationWarningIn = {
	/** Enable or disable position deviation log messages. */
	enabled: boolean
	/** Optional value in the range [01], where 0 is no position deviation and 1 is the maximum position deviation (equivalent to the amount of position deviation that causes a protective stop of the robot). If no threshold is specified by the user, a default value of 0.8 is used. */
	threshold?: number
}

/** Reset the revolution counter, if no offset is specified. This is applied on joints which safety limits are set to "Unlimited" and are only applied when new safety settings are applied with limited joint angles. */
export type MotionResetRevolutionCounterIn = {
	/** Reset the revolution counter to one close to the given qNear joint vector. If not defined, the joint’s actual number of revolutions are used. */
	qNear?: number[]
}

/**
	* Servoj can be used for online realtime control of joint positions.
	* The gain parameter works the same way as the P-term of a PID controller, where it adjusts the current position towards the desired (q). The higher the gain, the faster reaction the robot will have.
	* The parameter lookahead_time is used to project the current position forward in time with the current velocity. A low value gives fast reaction, a high value prevents overshoot.
	* Note: A high gain or a short lookahead time may cause instability and vibrations. Especially if the target positions are noisy or updated at a low frequency.
	* It is preferred to call this function with a new setpoint (q) in each time step (thus the default t=0.008). */
export type MotionServoJIn = {
	/** Joint angles in radians representing rotations of base, shoulder, elbow, wrist1, wrist2, and wrist3. */
	q: number[]
	/** Not used in current version (reserved for future use). */
	a: number
	/** Not used in current version (reserved for future use). */
	v: number
	/** Time where the command is controlling the robot. The function is blocking for time t [s] (default: 0.008). */
	t?: number
	/** Time [s], range [0.03,0.2] smoothens the trajectory with this lookahead time (default: 0.1). */
	lookahead_time?: number
	/** Proportional gain for following target position, range [100,2000] (default: 300). */
	gain?: number
}

/** Deprecated: Tells the robot controller the tick count of the encoder. This function is useful for absolute encoders, use conveyor_pulse_decode() for setting up an incremental encoder. For circular conveyors, the value must be between 0 and the number of ticks per revolution. */
export type MotionSetConveyorTickCountIn = {
	/** Tick count of the conveyor (Integer). */
	tick_count: number
	/** Resolution of the encoder, needed to handle wrapping nicely (Integer).
	 * 0 is a 32 bit signed encoder, range [-2147483648  2147483647] (default).
	 * 1 is an 8 bit unsigned encoder, range [0  255].
	 * 2 is a 16 bit unsigned encoder, range [0  65535].
	 * 3 is a 24 bit unsigned encoder, range [0  16777215].
	 * 4 is a 32 bit unsigned encoder, range [0  4294967295]. */
	absolute_encoder_resolution?: number
}

/** Set joint positions of simulated robot. */
export type MotionSetPosIn = {
	/** Joint positions. */
	q: number[]
}

/** Sets the transition hardness between normal mode, reduced mode, and safeguard stop. */
export type MotionSetSafetyModeTransitionHardnessIn = {
	/** An integer specifying transition hardness.
	 * 0 is hard transition between modes using maximum torque, similar to emergency stop.
	 * 1 is soft transition between modes. */
	type: number
}

/**
	* Joint speed: Accelerate linearly in joint space and continue with constant joint speed.
	* The time t is optional if provided the function will return after time t, regardless of the target speed has been reached.
	* If the time t is not provided, the function will return when the target speed is reached. */
export type MotionSpeedJIn = {
	/** Joint speeds [rad/s]. */
	qd: number[]
	/** Joint acceleration [rad/s^2] (of leading axis). */
	a: number
	/** Time [s] before the function returns (optional). */
	t?: number
}

/**
	* Tool speed: Accelerate linearly in Cartesian space and continue with constant tool speed.
	* The time t is optional if provided the function will return after time t, regardless of the target speed has been reached.
	* If the time t is not provided, the function will return when the target speed is reached. */
export type MotionSpeedLIn = {
	/** Tool speed [m/s] (spatial vector). */
	xd: number[]
	/** Tool position acceleration [m/s^2]. */
	a: number
	/** Time [s] before function returns (optional). */
	t?: number
	/** Tool acceleration [rad/s^2] (optional), if not defined a, position acceleration, is used. */
	aRot?: string
}

/**
	* Stop tracking the conveyor, started by track_conveyor_linear() or track_conveyor_circular(),
	* and decelerate all joint speeds to zero. */
export type MotionStopConveyorTrackingIn = {
	/** Joint acceleration [rad/s^2] (optional). */
	a?: number
}

/** Stop (linear in joint space): Decelerate joint speeds to zero. */
export type MotionStopJIn = {
	/** Joint acceleration [rad/s^2] (of leading axis). */
	a: number
}

/** Stop (linear in tool space): Decelerate tool speed to zero. */
export type MotionStopLIn = {
	/** Tool acceleration [m/s^2]. */
	a: number
	/** Tool acceleration [rad/s^2] (optional), if not defined a, position acceleration, is used. */
	aRot?: string
}

/** Makes robot movement (movej() etc.) track a circular conveyor. */
export type MotionTrackConveyorCircularIn = {
	/** Pose vector that determines the center of the conveyor in the base coordinate system of the robot. */
	center: number[]
	/** How many ticks the encoder sees when the conveyor moves one revolution. */
	ticksPerRevolution: number
	/** Should the tool rotate with the conveyor or stay in the orientation specified by the trajectory (movel() etc.). */
	rotateTool: boolean
	/** The index of the encoder to associate with the conveyor tracking (optional, default is 0). */
	encoderIndex?: number
}

/** Makes robot movement (movej() etc.) track a linear conveyor. */
export type MotionTrackConveyorLinearIn = {
	/** Pose vector that determines the direction of the conveyor in the base coordinate system of the robot. */
	direction: number[]
	/** How many ticks the encoder sees when the conveyor moves one meter. */
	ticksPerMeter: number
	/** The index of the encoder to associate with the conveyor tracking (optional, default is 0). */
	encoderIndex?: number
}