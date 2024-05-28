namespace Schema.Socket.Index
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Information about all the robot sessions currently available
    /// </summary>
    public partial class SessionsOut
    {
        /// <summary>
        /// the capacity for new virtual robots
        /// </summary>
        [JsonProperty("capacity")]
        public double Capacity { get; set; }

        /// <summary>
        /// The available virtual robot sessions
        /// </summary>
        [JsonProperty("sessions")]
        public List<RobotSession> Sessions { get; set; }
    }

    /// <summary>
    /// Information about a robot session
    /// </summary>
    public partial class RobotSession
    {
        /// <summary>
        /// The address of the robot
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// The name of the robot
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The names of users in the session
        /// </summary>
        [JsonProperty("users")]
        public List<string> Users { get; set; }
    }

    /// <summary>
    /// Join Session Payload.
    /// </summary>
    public partial class JoinSessionOut
    {
        /// <summary>
        /// The address of the socket namespace this session runs on.
        /// </summary>
        [JsonProperty("robot")]
        public RobotInfo Robot { get; set; }

        /// <summary>
        /// The token needed to join the session.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }
    }

    /// <summary>
    /// Information about a real robot
    ///
    /// The address of the socket namespace this session runs on.
    /// </summary>
    public partial class RobotInfo
    {
        /// <summary>
        /// The address of the robot
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// The name of the robot
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
