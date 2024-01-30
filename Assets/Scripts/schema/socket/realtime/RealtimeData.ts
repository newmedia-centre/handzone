/**
 * Struct containing all of the data send using the realtime 5.10 protocol
 * https://s3-eu-west-1.amazonaws.com/ur-support-site/16496/ClientInterfaces_Realtime.pdf
 */
export interface RealtimeData {
    /**
     * Controller realtime thread execution time
     */
    controller_timer: number;
    /**
     * Current state of the digital inputs. NOTE: these are bits encoded as int64_t, e.g. a
     * value of 5 corresponds to bit 0 and bit 2 set high
     */
    digital_input_bits: number;
    /**
     * Digital outputs
     */
    digital_outputs: number;
    /**
     * Elbow position
     */
    elbow_position: number[];
    /**
     * Elbow velocity
     */
    elbow_velocity: number[];
    /**
     * Actual joint currents
     */
    i_actual: number[];
    /**
     * Joint control currents
     */
    i_control: number[];
    /**
     * Masterboard: Robot current
     */
    i_robot: number;
    /**
     * Target joint currents
     */
    i_target: number[];
    /**
     * Joint control modes
     */
    joint_modes: number[];
    /**
     * Norm of Cartesian linear momentum
     */
    linear_momentum_norm: number;
    /**
     * Target joint moments (torques)
     */
    m_target: number[];
    /**
     * Total message length in bytes
     */
    message_size: number;
    /**
     * Temperature of each joint in degrees celsius
     */
    motor_temperatures: number[];
    /**
     * Payload Center of Gravity (x, y, z) [m]
     */
    payload_cog: number[];
    /**
     * Payload Inertia (Ixx, Iyy, Izz, Ixy, Ixz, Iyz) [kg*m^2]
     */
    payload_inertia: number[];
    /**
     * Payload Mass [kg]
     */
    payload_mass: number;
    /**
     * Program state
     */
    program_state: number;
    /**
     * Actual joint positions
     */
    q_actual: number[];
    /**
     * Target joint positions
     */
    q_target: number[];
    /**
     * Actual joint velocities
     */
    qd_actual: number[];
    /**
     * Target joint velocities
     */
    qd_target: number[];
    /**
     * Target joint accelerations
     */
    qdd_target: number[];
    /**
     * Robot mode
     */
    robot_mode: number;
    /**
     * Safety mode
     */
    safety_mode: number;
    /**
     * Safety status
     */
    safety_status: number;
    /**
     * Speed scaling of the trajectory limiter
     */
    speed_scaling: number;
    /**
     * Generalised forces in the TCP
     */
    tcp_force: number[];
    /**
     * Actual speed of the tool given in Cartesian coordinates
     */
    tcp_speed_actual: number[];
    /**
     * Target speed of the tool given in Cartesian coordinates
     */
    tcp_speed_target: number[];
    /**
     * Time elapsed since the controller was started
     */
    time: number;
    /**
     * Tool x,y and z accelerometer values (software version 1.7)
     */
    tool_accelerometer_values: number[];
    /**
     * Actual Cartesian coordinates of the tool: (x,y,z,rx,ry,rz)
     */
    tool_vector_actual: number[];
    /**
     * Target Cartesian coordinates of the tool: (x,y,z,rx,ry,rz)
     */
    tool_vector_target: number[];
    /**
     * Actual joint voltages
     */
    v_actual: number[];
    /**
     * Masterboard: Main voltage
     */
    v_main: number;
    /**
     * Masterboard: Robot voltage (48V)
     */
    v_robot: number;
}
