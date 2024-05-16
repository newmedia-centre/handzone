namespace Schema.Socket.Index
{

    /// <summary>
    /// Information about all the robot sessions currently available
    /// </summary>
    public partial class RobotsOut
    {
        /// <summary>
        /// The real robot that the user is allowed to join, can be null
        /// </summary>
        public RobotInfo Real { get; set; }

        /// <summary>
        /// The available sessions
        /// </summary>
        public RobotSession[] Sessions { get; set; }
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
        public string Address { get; set; }

        /// <summary>
        /// The name of the robot
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The port of the robot
        /// </summary>
        public double Port { get; set; }

        /// <summary>
        /// The vnc port of the robot
        /// </summary>
        public double? Vnc { get; set; }
    }

    /// <summary>
    /// Information about a robot session
    /// </summary>
    public partial class RobotSession
    {
        /// <summary>
        /// The address of the session
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The type of the robot used for the session
        /// </summary>
        public TypeEnum Type { get; set; }

        /// <summary>
        /// The names of users in the session
        /// </summary>
        public string[] Users { get; set; }
    }

    /// <summary>
    /// Join Session Payload.
    /// </summary>
    public partial class JoinSessionOut
    {
        /// <summary>
        /// The address of the socket namespace this session runs on.
        /// </summary>
        public RobotInfo Robot { get; set; }

        /// <summary>
        /// The token needed to join the session.
        /// </summary>
        public string Token { get; set; }
    }

    /// <summary>
    /// The type of the robot used for the session
    /// </summary>
    public enum TypeEnum { Real, Virtual };
}
