namespace Schema.Socket
{

    /// <summary>
    /// Represents a 6D vector consisting of three force components and three torque components.
    /// </summary>
    public partial class Vector6D
    {
        /// <summary>
        /// U-axis
        /// </summary>
        public double U { get; set; }

        /// <summary>
        /// V-axis
        /// </summary>
        public double V { get; set; }

        /// <summary>
        /// W-axis
        /// </summary>
        public double W { get; set; }

        /// <summary>
        /// X-axis
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y-axis
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// Z-axis
        /// </summary>
        public double Z { get; set; }
    }
}
