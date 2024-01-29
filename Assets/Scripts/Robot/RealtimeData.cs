using Newtonsoft.Json;

public struct RealtimeData {
    // Total message length in bytes
    [JsonProperty("message_size")]
    public int MessageSize;

    // Time elapsed since the controller was started
    [JsonProperty("time")]
    public double Time;

    // Target joint positions
    [JsonProperty("q_target")]
    public double[] QTarget;

    // Target joint velocities
    [JsonProperty("qd_target")]
    public double[] QdTarget;

    // Target joint accelerations
    [JsonProperty("qdd_target")]
    public double[] QddTarget;

    // Target joint currents
    [JsonProperty("i_target")]
    public double[] ITarget;

    // Target joint moments (torques)
    [JsonProperty("m_target")]
    public double[] MTarget;

    // Actual joint positions
    [JsonProperty("q_actual")]
    public double[] QActual;

    // Actual joint velocities
    [JsonProperty("qd_actual")]
    public double[] QdActual;

    // Actual joint currents
    [JsonProperty("i_actual")]
    public double[] IActual;

    // Joint control currents
    [JsonProperty("i_control")]
    public double[] IControl;

    // Actual Cartesian coordinates of the tool: (x, y, z, rx, ry, rz)
    [JsonProperty("tool_vector_actual")]
    public double[] ToolVectorActual;

    // Actual speed of the tool given in Cartesian coordinates
    [JsonProperty("tcp_speed_actual")]
    public double[] TcpSpeedActual;

    // Generalized forces in the TCP
    [JsonProperty("tcp_force")]
    public double[] TcpForce;

    // Target Cartesian coordinates of the tool: (x, y, z, rx, ry, rz)
    [JsonProperty("tool_vector_target")]
    public double[] ToolVectorTarget;

    // Target speed of the tool given in Cartesian coordinates
    [JsonProperty("tcp_speed_target")]
    public double[] TcpSpeedTarget;

    // Current state of the digital inputs
    // NOTE: these are bits encoded as int64_t, e.g., a value of 5 corresponds to bit 0 and bit 2 set high
    [JsonProperty("digital_input_bits")]
    public double DigitalInputBits;

    // Temperature of each joint in degrees celsius
    [JsonProperty("motor_temperatures")]
    public double[] MotorTemperatures;

    // Controller realtime thread execution time
    [JsonProperty("controller_timer")]
    public double ControllerTimer;

    // Robot mode
    [JsonProperty("robot_mode")]
    public double RobotMode;

    // Joint control modes
    [JsonProperty("joint_modes")]
    public double[] JointModes;

    // Safety mode
    [JsonProperty("safety_mode")]
    public double SafetyMode;

    // Tool x, y, and z accelerometer values (software version 1.7)
    [JsonProperty("tool_accelerometer_values")]
    public double[] ToolAccelerometerValues;

    // Speed scaling of the trajectory limiter
    [JsonProperty("speed_scaling")]
    public double SpeedScaling;

    // Norm of Cartesian linear momentum
    [JsonProperty("linear_momentum_norm")]
    public double LinearMomentumNorm;

    // Masterboard: Main voltage
    [JsonProperty("v_main")]
    public double VMain;

    // Masterboard: Robot voltage (48V)
    [JsonProperty("v_robot")]
    public double VRobot;

    // Masterboard: Robot current
    [JsonProperty("i_robot")]
    public double IRobot;

    // Actual joint voltages
    [JsonProperty("v_actual")]
    public double[] VActual;

    // Digital outputs
    [JsonProperty("digital_outputs")]
    public double DigitalOutputs;

    // Program state
    [JsonProperty("program_state")]
    public double ProgramState;

    // Elbow position
    [JsonProperty("elbow_position")]
    public double[] ElbowPosition;

    // Elbow velocity
    [JsonProperty("elbow_velocity")]
    public double[] ElbowVelocity;

    // Safety status
    [JsonProperty("safety_status")]
    public double SafetyStatus;

    // Payload Mass [kg]
    [JsonProperty("payload_mass")]
    public double PayloadMass;

    // Payload Center of Gravity (x, y, z) [m]
    [JsonProperty("payload_cog")]
    public double[] PayloadCog;

    // Payload Inertia (Ixx, Iyy, Izz, Ixy, Ixz, Iyz) [kg*m^2]
    [JsonProperty("payload_inertia")]
    public double[] PayloadInertia;
}