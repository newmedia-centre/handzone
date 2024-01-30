namespace Schema.Socket.Realtime
{

    /// <summary>
    /// Struct containing all of the data send using the realtime 5.10 protocol
    /// https://s3-eu-west-1.amazonaws.com/ur-support-site/16496/ClientInterfaces_Realtime.pdf
    /// </summary>
    public partial class RealtimeData
    {
        /// <summary>
        /// Controller realtime thread execution time
        /// </summary>
        public double ControllerTimer { get; set; }

        /// <summary>
        /// Current state of the digital inputs. NOTE: these are bits encoded as int64_t, e.g. a
        /// value of 5 corresponds to bit 0 and bit 2 set high
        /// </summary>
        public double DigitalInputBits { get; set; }

        /// <summary>
        /// Digital outputs
        /// </summary>
        public double DigitalOutputs { get; set; }

        /// <summary>
        /// Elbow position
        /// </summary>
        public double[] ElbowPosition { get; set; }

        /// <summary>
        /// Elbow velocity
        /// </summary>
        public double[] ElbowVelocity { get; set; }

        /// <summary>
        /// Actual joint currents
        /// </summary>
        public double[] IActual { get; set; }

        /// <summary>
        /// Joint control currents
        /// </summary>
        public double[] IControl { get; set; }

        /// <summary>
        /// Masterboard: Robot current
        /// </summary>
        public double IRobot { get; set; }

        /// <summary>
        /// Target joint currents
        /// </summary>
        public double[] ITarget { get; set; }

        /// <summary>
        /// Joint control modes
        /// </summary>
        public double[] JointModes { get; set; }

        /// <summary>
        /// Norm of Cartesian linear momentum
        /// </summary>
        public double LinearMomentumNorm { get; set; }

        /// <summary>
        /// Target joint moments (torques)
        /// </summary>
        public double[] MTarget { get; set; }

        /// <summary>
        /// Total message length in bytes
        /// </summary>
        public double MessageSize { get; set; }

        /// <summary>
        /// Temperature of each joint in degrees celsius
        /// </summary>
        public double[] MotorTemperatures { get; set; }

        /// <summary>
        /// Payload Center of Gravity (x, y, z) [m]
        /// </summary>
        public double[] PayloadCog { get; set; }

        /// <summary>
        /// Payload Inertia (Ixx, Iyy, Izz, Ixy, Ixz, Iyz) [kg*m^2]
        /// </summary>
        public double[] PayloadInertia { get; set; }

        /// <summary>
        /// Payload Mass [kg]
        /// </summary>
        public double PayloadMass { get; set; }

        /// <summary>
        /// Program state
        /// </summary>
        public double ProgramState { get; set; }

        /// <summary>
        /// Actual joint positions
        /// </summary>
        public double[] QActual { get; set; }

        /// <summary>
        /// Target joint positions
        /// </summary>
        public double[] QTarget { get; set; }

        /// <summary>
        /// Actual joint velocities
        /// </summary>
        public double[] QdActual { get; set; }

        /// <summary>
        /// Target joint velocities
        /// </summary>
        public double[] QdTarget { get; set; }

        /// <summary>
        /// Target joint accelerations
        /// </summary>
        public double[] QddTarget { get; set; }

        /// <summary>
        /// Robot mode
        /// </summary>
        public double RobotMode { get; set; }

        /// <summary>
        /// Safety mode
        /// </summary>
        public double SafetyMode { get; set; }

        /// <summary>
        /// Safety status
        /// </summary>
        public double SafetyStatus { get; set; }

        /// <summary>
        /// Speed scaling of the trajectory limiter
        /// </summary>
        public double SpeedScaling { get; set; }

        /// <summary>
        /// Generalised forces in the TCP
        /// </summary>
        public double[] TcpForce { get; set; }

        /// <summary>
        /// Actual speed of the tool given in Cartesian coordinates
        /// </summary>
        public double[] TcpSpeedActual { get; set; }

        /// <summary>
        /// Target speed of the tool given in Cartesian coordinates
        /// </summary>
        public double[] TcpSpeedTarget { get; set; }

        /// <summary>
        /// Time elapsed since the controller was started
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// Tool x,y and z accelerometer values (software version 1.7)
        /// </summary>
        public double[] ToolAccelerometerValues { get; set; }

        /// <summary>
        /// Actual Cartesian coordinates of the tool: (x,y,z,rx,ry,rz)
        /// </summary>
        public double[] ToolVectorActual { get; set; }

        /// <summary>
        /// Target Cartesian coordinates of the tool: (x,y,z,rx,ry,rz)
        /// </summary>
        public double[] ToolVectorTarget { get; set; }

        /// <summary>
        /// Actual joint voltages
        /// </summary>
        public double[] VActual { get; set; }

        /// <summary>
        /// Masterboard: Main voltage
        /// </summary>
        public double VMain { get; set; }

        /// <summary>
        /// Masterboard: Robot voltage (48V)
        /// </summary>
        public double VRobot { get; set; }
    }
}
