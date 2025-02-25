namespace Schema.Socket.Realtime
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The raw binary data from the robot
    /// </summary>
    public partial class RealtimeRawOut
    {
        /// <summary>
        /// The binary data, encoded as a base64 string.
        /// </summary>
        [JsonProperty("raw")]
        public string Raw { get; set; }
    }

    /// <summary>
    /// The parsed realtime data from the robot
    /// </summary>
    public partial class RealtimeDataOut
    {
        /// <summary>
        /// Controller realtime thread execution time
        /// </summary>
        [JsonProperty("controller_timer")]
        public double ControllerTimer { get; set; }

        /// <summary>
        /// Current state of the digital inputs. NOTE: these are bits encoded as int64_t, e.g. a
        /// value of 5 corresponds to bit 0 and bit 2 set high
        /// </summary>
        [JsonProperty("digital_input_bits")]
        public double DigitalInputBits { get; set; }

        /// <summary>
        /// Digital outputs
        /// </summary>
        [JsonProperty("digital_outputs")]
        public double DigitalOutputs { get; set; }

        /// <summary>
        /// Elbow position
        /// </summary>
        [JsonProperty("elbow_position")]
        public List<double> ElbowPosition { get; set; }

        /// <summary>
        /// Elbow velocity
        /// </summary>
        [JsonProperty("elbow_velocity")]
        public List<double> ElbowVelocity { get; set; }

        /// <summary>
        /// Actual joint currents
        /// </summary>
        [JsonProperty("i_actual")]
        public List<double> IActual { get; set; }

        /// <summary>
        /// Joint control currents
        /// </summary>
        [JsonProperty("i_control")]
        public List<double> IControl { get; set; }

        /// <summary>
        /// Masterboard: Robot current
        /// </summary>
        [JsonProperty("i_robot")]
        public double IRobot { get; set; }

        /// <summary>
        /// Target joint currents
        /// </summary>
        [JsonProperty("i_target")]
        public List<double> ITarget { get; set; }

        /// <summary>
        /// Joint control modes
        /// </summary>
        [JsonProperty("joint_modes")]
        public List<double> JointModes { get; set; }

        /// <summary>
        /// Norm of Cartesian linear momentum
        /// </summary>
        [JsonProperty("linear_momentum_norm")]
        public double LinearMomentumNorm { get; set; }

        /// <summary>
        /// Target joint moments (torques)
        /// </summary>
        [JsonProperty("m_target")]
        public List<double> MTarget { get; set; }

        /// <summary>
        /// Total message length in bytes
        /// </summary>
        [JsonProperty("message_size")]
        public double MessageSize { get; set; }

        /// <summary>
        /// Temperature of each joint in degrees celsius
        /// </summary>
        [JsonProperty("motor_temperatures")]
        public List<double> MotorTemperatures { get; set; }

        /// <summary>
        /// Payload Center of Gravity (x, y, z) [m]
        /// </summary>
        [JsonProperty("payload_cog")]
        public List<double> PayloadCog { get; set; }

        /// <summary>
        /// Payload Inertia (Ixx, Iyy, Izz, Ixy, Ixz, Iyz) [kg*m^2]
        /// </summary>
        [JsonProperty("payload_inertia")]
        public List<double> PayloadInertia { get; set; }

        /// <summary>
        /// Payload Mass [kg]
        /// </summary>
        [JsonProperty("payload_mass")]
        public double PayloadMass { get; set; }

        /// <summary>
        /// Program state
        /// </summary>
        [JsonProperty("program_state")]
        public double ProgramState { get; set; }

        /// <summary>
        /// Actual joint positions
        /// </summary>
        [JsonProperty("q_actual")]
        public List<double> QActual { get; set; }

        /// <summary>
        /// Target joint positions
        /// </summary>
        [JsonProperty("q_target")]
        public List<double> QTarget { get; set; }

        /// <summary>
        /// Actual joint velocities
        /// </summary>
        [JsonProperty("qd_actual")]
        public List<double> QdActual { get; set; }

        /// <summary>
        /// Target joint velocities
        /// </summary>
        [JsonProperty("qd_target")]
        public List<double> QdTarget { get; set; }

        /// <summary>
        /// Target joint accelerations
        /// </summary>
        [JsonProperty("qdd_target")]
        public List<double> QddTarget { get; set; }

        /// <summary>
        /// Robot mode
        /// </summary>
        [JsonProperty("robot_mode")]
        public double RobotMode { get; set; }

        /// <summary>
        /// Safety mode
        /// </summary>
        [JsonProperty("safety_mode")]
        public double SafetyMode { get; set; }

        /// <summary>
        /// Safety status
        /// </summary>
        [JsonProperty("safety_status")]
        public double SafetyStatus { get; set; }

        /// <summary>
        /// Speed scaling of the trajectory limiter
        /// </summary>
        [JsonProperty("speed_scaling")]
        public double SpeedScaling { get; set; }

        /// <summary>
        /// Generalised forces in the TCP
        /// </summary>
        [JsonProperty("tcp_force")]
        public List<double> TcpForce { get; set; }

        /// <summary>
        /// Actual speed of the tool given in Cartesian coordinates
        /// </summary>
        [JsonProperty("tcp_speed_actual")]
        public List<double> TcpSpeedActual { get; set; }

        /// <summary>
        /// Target speed of the tool given in Cartesian coordinates
        /// </summary>
        [JsonProperty("tcp_speed_target")]
        public List<double> TcpSpeedTarget { get; set; }

        /// <summary>
        /// Time elapsed since the controller was started
        /// </summary>
        [JsonProperty("time")]
        public double Time { get; set; }

        /// <summary>
        /// Tool x,y and z accelerometer values (software version 1.7)
        /// </summary>
        [JsonProperty("tool_accelerometer_values")]
        public List<double> ToolAccelerometerValues { get; set; }

        /// <summary>
        /// Actual Cartesian coordinates of the tool: (x,y,z,rx,ry,rz)
        /// </summary>
        [JsonProperty("tool_vector_actual")]
        public List<double> ToolVectorActual { get; set; }

        /// <summary>
        /// Target Cartesian coordinates of the tool: (x,y,z,rx,ry,rz)
        /// </summary>
        [JsonProperty("tool_vector_target")]
        public List<double> ToolVectorTarget { get; set; }

        /// <summary>
        /// Actual joint voltages
        /// </summary>
        [JsonProperty("v_actual")]
        public List<double> VActual { get; set; }

        /// <summary>
        /// Masterboard: Main voltage
        /// </summary>
        [JsonProperty("v_main")]
        public double VMain { get; set; }

        /// <summary>
        /// Masterboard: Robot voltage (48V)
        /// </summary>
        [JsonProperty("v_robot")]
        public double VRobot { get; set; }
    }
}
