namespace Schema.Socket.Internals
{

    /// <summary>
    /// Calculate the inverse kinematics for a given pose.
    /// </summary>
    public partial class InternalsGetInverseKinIn
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

    public partial class InternalsGetInverseKinCallback
    {
        /// <summary>
        /// The inverse kinematics positions
        /// </summary>
        public double[] Ik { get; set; }
    }
}
