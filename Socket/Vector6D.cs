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
        public float U { get; set; }

        /// <summary>
        /// V-axis
        /// </summary>
        public float V { get; set; }

        /// <summary>
        /// W-axis
        /// </summary>
        public float W { get; set; }

        /// <summary>
        /// X-axis
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y-axis
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Z-axis
        /// </summary>
        public float Z { get; set; }
    }
}
