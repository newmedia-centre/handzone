namespace Schema.Socket.Unity
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

    /// <summary>
    /// Sends a message from one Unity client to another Unity client.
    /// </summary>
    public partial class UnityMessageIn
    {
        /// <summary>
        /// The message content.
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Allows the Unity client to send its current position
    /// </summary>
    public partial class UnityPlayerIn
    {
        /// <summary>
        /// The player's head-mounted display position.
        /// </summary>
        public SixDofPosition Hmd { get; set; }

        /// <summary>
        /// The player's left hand position.
        /// </summary>
        public SixDofPosition Left { get; set; }

        /// <summary>
        /// The player's right hand position.
        /// </summary>
        public SixDofPosition Right { get; set; }
    }

    /// <summary>
    /// Represents a 6 degrees of freedom position with position and rotation vectors.
    ///
    /// The player's head-mounted display position.
    ///
    /// The player's left hand position.
    ///
    /// The player's right hand position.
    /// </summary>
    public partial class SixDofPosition
    {
        /// <summary>
        /// The position vector.
        /// </summary>
        public Vector3D Position { get; set; }

        /// <summary>
        /// The rotation vector.
        /// </summary>
        public Vector3D Rotation { get; set; }
    }

    /// <summary>
    /// Represents a 3D vector consisting of three components
    ///
    /// The position vector.
    ///
    /// The rotation vector.
    ///
    /// The pendant's position.
    ///
    /// The pendant's rotation.
    /// </summary>
    public partial class Vector3D
    {
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

    /// <summary>
    /// Grabs ownership and sends the position of the pendant.
    /// </summary>
    public partial class UnityPendantIn
    {
        /// <summary>
        /// The pendant's position.
        /// </summary>
        public Vector3D Position { get; set; }

        /// <summary>
        /// The pendant's rotation.
        /// </summary>
        public Vector3D Rotation { get; set; }
    }

    /// <summary>
    /// Sends a message from one Unity client to another Unity client.
    /// </summary>
    public partial class UnityMessageOut
    {
        /// <summary>
        /// The message content.
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Sends an array of all the Unity clients' positions.
    /// </summary>
    public partial class UnityPlayersOut
    {
        /// <summary>
        /// An array of player data.
        /// </summary>
        public PlayerData[] Players { get; set; }
    }

    /// <summary>
    /// Represents the data of a player.
    /// </summary>
    public partial class PlayerData
    {
        /// <summary>
        /// The player's color.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// The player's head-mounted display position.
        /// </summary>
        public SixDofPosition Hmd { get; set; }

        /// <summary>
        /// The player's ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The player's left hand position.
        /// </summary>
        public SixDofPosition Left { get; set; }

        /// <summary>
        /// The player's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The player's right hand position.
        /// </summary>
        public SixDofPosition Right { get; set; }
    }

    /// <summary>
    /// Sends the current position of the pendant.
    /// </summary>
    public partial class UnityPendantOut
    {
        /// <summary>
        /// The owner of the pendant.
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// The pendant's position.
        /// </summary>
        public Vector3D Position { get; set; }

        /// <summary>
        /// The pendant's rotation.
        /// </summary>
        public Vector3D Rotation { get; set; }
    }
}
