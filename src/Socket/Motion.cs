namespace Schema.Socket.Motion
{

    /// <summary>
    /// Deprecated: Tells the robot controller to treat digital inputs number A and B as pulses
    /// for a conveyor encoder. Only digital input 0, 1, 2, or 3 can be used. This function is
    /// replaced by encoder_enable_pulse_decode and should therefore not be used moving forward.
    /// </summary>
    public partial class MotionConveyorPulseDecodeIn
    {
        /// <summary>
        /// Encoder input A, values of 0-3 are the digital inputs 0-3.
        /// </summary>
        public double A { get; set; }

        /// <summary>
        /// Encoder input B, values of 0-3 are the digital inputs 0-3.
        /// </summary>
        public double B { get; set; }

        /// <summary>
        /// An integer determining how to treat the inputs on A and B: 0 - No encoder, pulse decoding
        /// is disabled. 1 - Quadrature encoder, input A and B must be square waves with 90 degree
        /// offset. Direction of the conveyor can be determined. 2 - Rising and falling edge on
        /// single input (A). 3 - Rising edge on single input (A). 4 - Falling edge on single input
        /// (A). The controller can decode inputs at up to 40kHz.
        /// </summary>
        public double Type { get; set; }
    }

    /// <summary>
    /// Sets up an encoder hooked up to the pulse decoder of the controller.
    /// </summary>
    public partial class MotionEncoderEnablePulseDecodeIn
    {
        /// <summary>
        /// Encoder input A, values of 0-3 are the digital inputs 0-3.
        /// </summary>
        public double A { get; set; }

        /// <summary>
        /// Encoder input B, values of 0-3 are the digital inputs 0-3.
        /// </summary>
        public double B { get; set; }

        /// <summary>
        /// An integer determining how to treat the inputs on A and B: 0 - No encoder, pulse decoding
        /// is disabled. 1 - Quadrature encoder, input A and B must be square waves with 90 degree
        /// offset. Direction of the conveyor can be determined. 2 - Rising and falling edge on
        /// single input (A). 3 - Rising edge on single input (A). 4 - Falling edge on single input
        /// (A). The controller can decode inputs at up to 40kHz.
        /// </summary>
        public double DecoderType { get; set; }

        /// <summary>
        /// Index of the encoder to define. Must be either 0 or 1.
        /// </summary>
        public double EncoderIndex { get; set; }
    }

    /// <summary>
    /// Sets up an encoder expecting to be updated with tick counts via the function
    /// encoder_set_tick_count.
    /// </summary>
    public partial class MotionEncoderEnableSetTickCountIn
    {
        /// <summary>
        /// Index of the encoder to define. Must be either 0 or 1.
        /// </summary>
        public double EncoderIndex { get; set; }

        /// <summary>
        /// Range of the encoder (integer). Needed to handle wrapping nicely. 0 - 32 bit signed
        /// encoder, range [-2147483648  2147483647]. 1 - 8 bit unsigned encoder, range [0  255]. 2 -
        /// 16 bit unsigned encoder, range [0  65535]. 3 - 24 bit unsigned encoder, range [0
        /// 16777215]. 4 - 32 bit unsigned encoder, range [0  4294967295].
        /// </summary>
        public double RangeId { get; set; }
    }

    /// <summary>
    /// Returns the tick count of the designated encoder.
    /// </summary>
    public partial class MotionEncoderGetTickCountIn
    {
        /// <summary>
        /// Index of the encoder to query. Must be either 0 or 1.
        /// </summary>
        public double EncoderIndex { get; set; }
    }

    public partial class MotionEncoderGetTickCountCallback
    {
        /// <summary>
        /// The conveyor encoder tick count (float).
        /// </summary>
        public double Count { get; set; }
    }

    /// <summary>
    /// Sets the active tcp offset, i.e. the transformation from the output flange coordinate
    /// system to the TCP as a pose.
    /// </summary>
    public partial class MotionSetTcpIn
    {
        /// <summary>
        /// The TCP pose to set.
        /// </summary>
        public double[] Pose { get; set; }
    }

    /// <summary>
    /// Tells the robot controller the tick count of the encoder. This function is useful for
    /// absolute encoders (e.g., MODBUS). Assumes that the encoder is enabled using
    /// encoder_enable_set_tick_count first.
    /// </summary>
    public partial class MotionEncoderSetTickCountIn
    {
        /// <summary>
        /// The tick count to set. Must be within the range of the encoder.
        /// </summary>
        public double Count { get; set; }

        /// <summary>
        /// Index of the encoder to define. Must be either 0 or 1.
        /// </summary>
        public double EncoderIndex { get; set; }
    }

    /// <summary>
    /// Returns the delta_tick_count. Unwinds in case encoder wraps around the range. If no
    /// wrapping has happened, the given delta_tick_count is returned without any modification.
    /// This heuristic function checks that a given delta_tick_count value is reasonable. If the
    /// encoder wrapped around the end of the range, it compensates (i.e., unwinds) and returns
    /// the adjusted result. If a delta_tick_count is larger than half the range of the encoder,
    /// wrapping is assumed and is compensated. As a consequence, this function only works when
    /// the range of the encoder is explicitly known, and therefore the designated encoder must
    /// be enabled. If not, this function will always return nil.
    /// </summary>
    public partial class MotionEncoderUnwindDeltaTickCountIn
    {
        /// <summary>
        /// The delta (difference between two) tick count to unwind (float).
        /// </summary>
        public double DeltaTickCount { get; set; }

        /// <summary>
        /// Index of the encoder to query. Must be either 0 or 1.
        /// </summary>
        public double EncoderIndex { get; set; }
    }

    /// <summary>
    /// Set robot to be controlled in force mode.
    /// </summary>
    public partial class MotionForceModeIn
    {
        /// <summary>
        /// (Float) 6d vector. For compliant axes, these values are the maximum allowed tcp speed
        /// along/about the axis.
        /// </summary>
        public double[] Limits { get; set; }

        /// <summary>
        /// A 6d vector of 0s and 1s. 1 means that the robot will be compliant in the corresponding
        /// axis of the task frame.
        /// </summary>
        public double[] SelectionVector { get; set; }

        /// <summary>
        /// A pose vector that defines the force frame relative to the base frame.
        /// </summary>
        public double[] TaskFrame { get; set; }

        /// <summary>
        /// An integer [13] specifying how the robot interprets the force frame.
        /// </summary>
        public double Type { get; set; }

        /// <summary>
        /// The forces/torques the robot will apply to its environment.
        /// </summary>
        public double[] Wrench { get; set; }
    }

    /// <summary>
    /// Sets the damping parameter in force mode.
    /// </summary>
    public partial class MotionForceModeSetDampingIn
    {
        /// <summary>
        /// Between 0 and 1, default value is 0.
        /// </summary>
        public double Damping { get; set; }
    }

    /// <summary>
    /// Deprecated: Tells the tick count of the encoder. Note that the controller interpolates
    /// tick counts to get more accurate movements with low resolution encoders. Deprecated: This
    /// function is replaced by encoder_get_tick_count and it should therefore not be used moving
    /// forward.
    /// </summary>
    public partial class MotionGetConveyorTickCountCallback
    {
        /// <summary>
        /// The conveyor encoder tick count.
        /// </summary>
        public double Count { get; set; }
    }

    /// <summary>
    /// Query the target TCP pose as given by the trajectory being followed. This script function
    /// is useful in conjunction with conveyor tracking to know what the target pose of the TCP
    /// would be if no offset was applied.
    /// </summary>
    public partial class MotionGetTargetTcpPoseAlongPathCallback
    {
        /// <summary>
        /// Target TCP pose.
        /// </summary>
        public double[] Pose { get; set; }
    }

    /// <summary>
    /// Query the target TCP speed as given by the trajectory being followed. This script
    /// function is useful in conjunction with conveyor tracking to know what the target speed of
    /// the TCP would be if no offset was applied.
    /// </summary>
    public partial class MotionGetTargetTcpSpeedAlongPathCallback
    {
        /// <summary>
        /// Target TCP speed as a vector.
        /// </summary>
        public double[] Speed { get; set; }
    }

    /// <summary>
    /// Move Circular: Move to position (circular in tool-space). TCP moves on the circular arc
    /// segment from current pose, through pose_via to pose_to. Accelerates to and moves with
    /// constant tool speed v. Use the mode parameter to define the orientation interpolation.
    /// </summary>
    public partial class MotionMoveCIn
    {
        /// <summary>
        /// Tool acceleration [m/s^2] (default: 1.2).
        /// </summary>
        public double A { get; set; }

        /// <summary>
        /// 0: Unconstrained mode. Interpolate orientation from current pose to target pose
        /// (pose_to). 1: Fixed mode. Keep orientation constant relative to the tangent of the
        /// circular arc (starting from current pose).
        /// </summary>
        public double Mode { get; set; }

        /// <summary>
        /// Target pose (note: only position is used in Fixed orientation mode). Pose_to can also be
        /// specified as joint positions, then forward kinematics is used to calculate the
        /// corresponding pose.
        /// </summary>
        public double[] PoseTo { get; set; }

        /// <summary>
        /// Path point (note: only position is used). Pose_via can also be specified as joint
        /// positions, then forward kinematics is used to calculate the corresponding pose.
        /// </summary>
        public double[] PoseVia { get; set; }

        /// <summary>
        /// Blend radius (of target pose) [m] (default: 0).
        /// </summary>
        public double R { get; set; }

        /// <summary>
        /// Tool speed [m/s] (default: 0.25).
        /// </summary>
        public double V { get; set; }
    }

    /// <summary>
    /// Move to position (linear in joint-space). When using this command, the robot must be at a
    /// standstill or come from a movej or movel with a blend. The speed and acceleration
    /// parameters control the trapezoid speed profile of the move. Alternatively, the t
    /// parameter can be used to set the time for this move. Time setting has priority over speed
    /// and acceleration settings.
    /// </summary>
    public partial class MotionMoveJIn
    {
        /// <summary>
        /// Joint acceleration of leading axis [rad/s^2] (default: 1.4).
        /// </summary>
        public double A { get; set; }

        /// <summary>
        /// Joint positions (q can also be specified as a pose, then inverse kinematics is used to
        /// calculate the corresponding joint positions).
        /// </summary>
        public double[] Q { get; set; }

        /// <summary>
        /// Blend radius [m] (default: 0). If a blend radius is set, the robot arm trajectory will be
        /// modified to avoid the robot stopping at the point. However, if the blend region of this
        /// move overlaps with the blend radius of previous or following waypoints, this move will be
        /// skipped, and an 'Overlapping Blends' warning message will be generated.
        /// </summary>
        public double R { get; set; }

        /// <summary>
        /// Time [s] (default: 0).
        /// </summary>
        public double T { get; set; }

        /// <summary>
        /// Joint speed of leading axis [rad/s] (default: 1.05).
        /// </summary>
        public double V { get; set; }
    }

    /// <summary>
    /// Move to position (linear in tool-space).
    /// </summary>
    public partial class MotionMoveLIn
    {
        /// <summary>
        /// Tool acceleration [m/s^2] (default: 1.2).
        /// </summary>
        public double A { get; set; }

        /// <summary>
        /// Target pose (pose can also be specified as joint positions, then forward kinematics is
        /// used to calculate the corresponding pose).
        /// </summary>
        public double[] Pose { get; set; }

        /// <summary>
        /// Blend radius [m] (default: 0).
        /// </summary>
        public double R { get; set; }

        /// <summary>
        /// Time [s] (default: 0).
        /// </summary>
        public double T { get; set; }

        /// <summary>
        /// Tool speed [m/s] (default: 0.25).
        /// </summary>
        public double V { get; set; }
    }

    /// <summary>
    /// Move Process: Blend circular (in tool-space) and move linear (in tool-space) to position.
    /// Accelerates to and moves with constant tool speed v.
    /// </summary>
    public partial class MotionMovePIn
    {
        /// <summary>
        /// Tool acceleration [m/s^2] (default: 1.2).
        /// </summary>
        public double A { get; set; }

        /// <summary>
        /// Target pose (pose can also be specified as joint positions, then forward kinematics is
        /// used to calculate the corresponding pose).
        /// </summary>
        public double[] Pose { get; set; }

        /// <summary>
        /// Blend radius [m] (default: 0).
        /// </summary>
        public double R { get; set; }

        /// <summary>
        /// Tool speed [m/s] (default: 0.25).
        /// </summary>
        public double V { get; set; }
    }

    /// <summary>
    /// Makes the robot pause if the specified error code occurs. The robot will only pause
    /// during program execution. This setting is reset when the program is stopped - call the
    /// command again before/during program execution to re-enable it.
    /// </summary>
    public partial class MotionPauseOnErrorCodeIn
    {
        /// <summary>
        /// The argument of the error. If this parameter is omitted, the robot will pause on any
        /// argument for the specified error code.
        /// </summary>
        public double? Argument { get; set; }

        /// <summary>
        /// The code of the error for which the robot should pause.
        /// </summary>
        public double Code { get; set; }
    }

    /// <summary>
    /// When enabled, this function generates warning messages to the log when the robot deviates
    /// from the target position. This function can be called at any point in the execution of a
    /// program. It has no return value.
    /// </summary>
    public partial class MotionPositionDeviationWarningIn
    {
        /// <summary>
        /// Enable or disable position deviation log messages.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Optional value in the range [01], where 0 is no position deviation and 1 is the maximum
        /// position deviation (equivalent to the amount of position deviation that causes a
        /// protective stop of the robot). If no threshold is specified by the user, a default value
        /// of 0.8 is used.
        /// </summary>
        public double? Threshold { get; set; }
    }

    /// <summary>
    /// Reset the revolution counter, if no offset is specified. This is applied on joints which
    /// safety limits are set to "Unlimited" and are only applied when new safety settings are
    /// applied with limited joint angles.
    /// </summary>
    public partial class MotionResetRevolutionCounterIn
    {
        /// <summary>
        /// Reset the revolution counter to one close to the given qNear joint vector. If not
        /// defined, the joint’s actual number of revolutions are used.
        /// </summary>
        public double[] QNear { get; set; }
    }

    /// <summary>
    /// Servoj can be used for online realtime control of joint positions. The gain parameter
    /// works the same way as the P-term of a PID controller, where it adjusts the current
    /// position towards the desired (q). The higher the gain, the faster reaction the robot will
    /// have. The parameter lookahead_time is used to project the current position forward in
    /// time with the current velocity. A low value gives fast reaction, a high value prevents
    /// overshoot. Note: A high gain or a short lookahead time may cause instability and
    /// vibrations. Especially if the target positions are noisy or updated at a low frequency.
    /// It is preferred to call this function with a new setpoint (q) in each time step (thus the
    /// default t=0.008).
    /// </summary>
    public partial class MotionServoJIn
    {
        /// <summary>
        /// Not used in current version (reserved for future use).
        /// </summary>
        public double A { get; set; }

        /// <summary>
        /// Proportional gain for following target position, range [100,2000] (default: 300).
        /// </summary>
        public double? Gain { get; set; }

        /// <summary>
        /// Time [s], range [0.03,0.2] smoothens the trajectory with this lookahead time (default:
        /// 0.1).
        /// </summary>
        public double? LookaheadTime { get; set; }

        /// <summary>
        /// Joint angles in radians representing rotations of base, shoulder, elbow, wrist1, wrist2,
        /// and wrist3.
        /// </summary>
        public double[] Q { get; set; }

        /// <summary>
        /// Time where the command is controlling the robot. The function is blocking for time t [s]
        /// (default: 0.008).
        /// </summary>
        public double? T { get; set; }

        /// <summary>
        /// Not used in current version (reserved for future use).
        /// </summary>
        public double V { get; set; }
    }

    /// <summary>
    /// Deprecated: Tells the robot controller the tick count of the encoder. This function is
    /// useful for absolute encoders, use conveyor_pulse_decode() for setting up an incremental
    /// encoder. For circular conveyors, the value must be between 0 and the number of ticks per
    /// revolution.
    /// </summary>
    public partial class MotionSetConveyorTickCountIn
    {
        /// <summary>
        /// Resolution of the encoder, needed to handle wrapping nicely (Integer). 0 is a 32 bit
        /// signed encoder, range [-2147483648  2147483647] (default). 1 is an 8 bit unsigned
        /// encoder, range [0  255]. 2 is a 16 bit unsigned encoder, range [0  65535]. 3 is a 24 bit
        /// unsigned encoder, range [0  16777215]. 4 is a 32 bit unsigned encoder, range [0
        /// 4294967295].
        /// </summary>
        public double? AbsoluteEncoderResolution { get; set; }

        /// <summary>
        /// Tick count of the conveyor (Integer).
        /// </summary>
        public double TickCount { get; set; }
    }

    /// <summary>
    /// Set joint positions of simulated robot.
    /// </summary>
    public partial class MotionSetPosIn
    {
        /// <summary>
        /// Joint positions.
        /// </summary>
        public double[] Q { get; set; }
    }

    /// <summary>
    /// Sets the transition hardness between normal mode, reduced mode, and safeguard stop.
    /// </summary>
    public partial class MotionSetSafetyModeTransitionHardnessIn
    {
        /// <summary>
        /// An integer specifying transition hardness. 0 is hard transition between modes using
        /// maximum torque, similar to emergency stop. 1 is soft transition between modes.
        /// </summary>
        public double Type { get; set; }
    }

    /// <summary>
    /// Joint speed: Accelerate linearly in joint space and continue with constant joint speed.
    /// The time t is optional if provided the function will return after time t, regardless of
    /// the target speed has been reached. If the time t is not provided, the function will
    /// return when the target speed is reached.
    /// </summary>
    public partial class MotionSpeedJIn
    {
        /// <summary>
        /// Joint acceleration [rad/s^2] (of leading axis).
        /// </summary>
        public double A { get; set; }

        /// <summary>
        /// Joint speeds [rad/s].
        /// </summary>
        public double[] Qd { get; set; }

        /// <summary>
        /// Time [s] before the function returns (optional).
        /// </summary>
        public double? T { get; set; }
    }

    /// <summary>
    /// Tool speed: Accelerate linearly in Cartesian space and continue with constant tool speed.
    /// The time t is optional if provided the function will return after time t, regardless of
    /// the target speed has been reached. If the time t is not provided, the function will
    /// return when the target speed is reached.
    /// </summary>
    public partial class MotionSpeedLIn
    {
        /// <summary>
        /// Tool position acceleration [m/s^2].
        /// </summary>
        public double A { get; set; }

        /// <summary>
        /// Tool acceleration [rad/s^2] (optional), if not defined a, position acceleration, is used.
        /// </summary>
        public string ARot { get; set; }

        /// <summary>
        /// Time [s] before function returns (optional).
        /// </summary>
        public double? T { get; set; }

        /// <summary>
        /// Tool speed [m/s] (spatial vector).
        /// </summary>
        public double[] Xd { get; set; }
    }

    /// <summary>
    /// Stop tracking the conveyor, started by track_conveyor_linear() or
    /// track_conveyor_circular(), and decelerate all joint speeds to zero.
    /// </summary>
    public partial class MotionStopConveyorTrackingIn
    {
        /// <summary>
        /// Joint acceleration [rad/s^2] (optional).
        /// </summary>
        public double? A { get; set; }
    }

    /// <summary>
    /// Stop (linear in joint space): Decelerate joint speeds to zero.
    /// </summary>
    public partial class MotionStopJIn
    {
        /// <summary>
        /// Joint acceleration [rad/s^2] (of leading axis).
        /// </summary>
        public double A { get; set; }
    }

    /// <summary>
    /// Stop (linear in tool space): Decelerate tool speed to zero.
    /// </summary>
    public partial class MotionStopLIn
    {
        /// <summary>
        /// Tool acceleration [m/s^2].
        /// </summary>
        public double A { get; set; }

        /// <summary>
        /// Tool acceleration [rad/s^2] (optional), if not defined a, position acceleration, is used.
        /// </summary>
        public string ARot { get; set; }
    }

    /// <summary>
    /// Makes robot movement (movej() etc.) track a circular conveyor.
    /// </summary>
    public partial class MotionTrackConveyorCircularIn
    {
        /// <summary>
        /// Pose vector that determines the center of the conveyor in the base coordinate system of
        /// the robot.
        /// </summary>
        public double[] Center { get; set; }

        /// <summary>
        /// The index of the encoder to associate with the conveyor tracking (optional, default is 0).
        /// </summary>
        public double? EncoderIndex { get; set; }

        /// <summary>
        /// Should the tool rotate with the conveyor or stay in the orientation specified by the
        /// trajectory (movel() etc.).
        /// </summary>
        public bool RotateTool { get; set; }

        /// <summary>
        /// How many ticks the encoder sees when the conveyor moves one revolution.
        /// </summary>
        public double TicksPerRevolution { get; set; }
    }

    /// <summary>
    /// Makes robot movement (movej() etc.) track a linear conveyor.
    /// </summary>
    public partial class MotionTrackConveyorLinearIn
    {
        /// <summary>
        /// Pose vector that determines the direction of the conveyor in the base coordinate system
        /// of the robot.
        /// </summary>
        public double[] Direction { get; set; }

        /// <summary>
        /// The index of the encoder to associate with the conveyor tracking (optional, default is 0).
        /// </summary>
        public double? EncoderIndex { get; set; }

        /// <summary>
        /// How many ticks the encoder sees when the conveyor moves one meter.
        /// </summary>
        public double TicksPerMeter { get; set; }
    }
}
