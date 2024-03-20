namespace Schema.Socket.Interfaces
{

    /// <summary>
    /// Set tool digital output signal level.
    /// </summary>
    public partial class InterfacesSetToolDigitalOutIn
    {
        /// <summary>
        /// The signal level (boolean).
        /// </summary>
        public bool B { get; set; }

        /// <summary>
        /// The number (id) of the output, integer: [0:1].
        /// </summary>
        public double N { get; set; }
    }

    /// <summary>
    /// Set standard digital output signal level.
    /// </summary>
    public partial class InterfacesSetStandardDigitalOutIn
    {
        /// <summary>
        /// The signal level (boolean).
        /// </summary>
        public bool B { get; set; }

        /// <summary>
        /// The number (id) of the output, integer: [0:7].
        /// </summary>
        public double N { get; set; }
    }

    /// <summary>
    /// Calculate the inverse kinematics for a given pose.
    /// </summary>
    public partial class InterfacesGetInverseKinIn
    {
        /// <summary>
        /// The maximum allowed position error. If not provided, the default value is used.
        /// </summary>
        public double? MaxPositionError { get; set; }

        /// <summary>
        /// The initial joint position for the inverse kinematics calculation. If not provided, the
        /// current joint position is used.
        /// </summary>
        public double[] Qnear { get; set; }

        /// <summary>
        /// The tool center point (TCP) to use for the inverse kinematics calculation. If not
        /// provided, the default TCP is used.
        /// </summary>
        public string TcpOffset { get; set; }

        /// <summary>
        /// Tool pose.
        /// </summary>
        public double[] X { get; set; }
    }

    public partial class InterfacesGetInverseKinCallback
    {
        /// <summary>
        /// The inverse kinematics positions
        /// </summary>
        public double[] Ik { get; set; }
    }
}
