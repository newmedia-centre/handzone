namespace Schema.Socket.Internals
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Calculate the inverse kinematics for a given pose.
    /// </summary>
    public partial class InternalsGetInverseKinIn
    {
        /// <summary>
        /// The maximum allowed position error. If not provided, the default value is used.
        /// </summary>
        [JsonProperty("maxPositionError", NullValueHandling = NullValueHandling.Ignore)]
        public double? MaxPositionError { get; set; }

        /// <summary>
        /// The initial joint position for the inverse kinematics calculation. If not provided, the
        /// current joint position is used.
        /// </summary>
        [JsonProperty("qnear", NullValueHandling = NullValueHandling.Ignore)]
        public List<double> Qnear { get; set; }

        /// <summary>
        /// The tool center point (TCP) to use for the inverse kinematics calculation. If not
        /// provided, the default TCP is used.
        /// </summary>
        [JsonProperty("tcp_offset", NullValueHandling = NullValueHandling.Ignore)]
        public List<double> TcpOffset { get; set; }

        /// <summary>
        /// Tool pose.
        /// </summary>
        [JsonProperty("x")]
        public List<double> X { get; set; }
    }

    public partial class InternalsGetInverseKinCallback
    {
        /// <summary>
        /// The inverse kinematics positions
        /// </summary>
        [JsonProperty("ik")]
        public List<double> Ik { get; set; }
    }
}
