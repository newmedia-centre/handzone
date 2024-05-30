namespace Schema.Socket.Unity
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Represents a 6D vector consisting of three force components and three torque components.
    /// </summary>
    public partial class Vector6D
    {
        /// <summary>
        /// U-axis
        /// </summary>
        [JsonProperty("u")]
        public double U { get; set; }

        /// <summary>
        /// V-axis
        /// </summary>
        [JsonProperty("v")]
        public double V { get; set; }

        /// <summary>
        /// W-axis
        /// </summary>
        [JsonProperty("w")]
        public double W { get; set; }

        /// <summary>
        /// X-axis
        /// </summary>
        [JsonProperty("x")]
        public double X { get; set; }

        /// <summary>
        /// Y-axis
        /// </summary>
        [JsonProperty("y")]
        public double Y { get; set; }

        /// <summary>
        /// Z-axis
        /// </summary>
        [JsonProperty("z")]
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
        [JsonProperty("message")]
        public string Message { get; set; }
    }

    /// <summary>
    /// Allows the Unity client to send its current position
    /// </summary>
    public partial class UnityPlayerIn
    {
        /// <summary>
        /// The player's cursor position on the pendant
        /// </summary>
        [JsonProperty("cursor")]
        public Vector2D Cursor { get; set; }

        /// <summary>
        /// The player's head-mounted display position.
        /// </summary>
        [JsonProperty("hmd")]
        public SixDofPosition Hmd { get; set; }

        /// <summary>
        /// The player's left hand position.
        /// </summary>
        [JsonProperty("left")]
        public SixDofPosition Left { get; set; }

        /// <summary>
        /// The player's right hand position.
        /// </summary>
        [JsonProperty("right")]
        public SixDofPosition Right { get; set; }
    }

    /// <summary>
    /// Represents a 2D vector consisting of two components
    ///
    /// The player's cursor position on the pendant
    /// </summary>
    public partial class Vector2D
    {
        /// <summary>
        /// X-axis
        /// </summary>
        [JsonProperty("x")]
        public double X { get; set; }

        /// <summary>
        /// Y-axis
        /// </summary>
        [JsonProperty("y")]
        public double Y { get; set; }
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
        [JsonProperty("position")]
        public Vector3D Position { get; set; }

        /// <summary>
        /// The rotation vector.
        /// </summary>
        [JsonProperty("rotation")]
        public Vector3D Rotation { get; set; }
    }

    /// <summary>
    /// Represents a 3D vector consisting of three components
    ///
    /// The position vector.
    ///
    /// The rotation vector.
    /// </summary>
    public partial class Vector3D
    {
        /// <summary>
        /// X-axis
        /// </summary>
        [JsonProperty("x")]
        public double X { get; set; }

        /// <summary>
        /// Y-axis
        /// </summary>
        [JsonProperty("y")]
        public double Y { get; set; }

        /// <summary>
        /// Z-axis
        /// </summary>
        [JsonProperty("z")]
        public double Z { get; set; }
    }

    /// <summary>
    /// Sends a message from one Unity client to another Unity client.
    /// </summary>
    public partial class UnityMessageOut
    {
        /// <summary>
        /// The message content.
        /// </summary>
        [JsonProperty("message")]
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
        [JsonProperty("players")]
        public List<PlayerData> Players { get; set; }
    }

    /// <summary>
    /// Represents the data of a player.
    /// </summary>
    public partial class PlayerData
    {
        /// <summary>
        /// The player's color.
        /// </summary>
        [JsonProperty("color")]
        public string Color { get; set; }

        /// <summary>
        /// The player's cursor position on the pendant
        /// </summary>
        [JsonProperty("cursor")]
        public Vector2D Cursor { get; set; }

        /// <summary>
        /// The player's head-mounted display position.
        /// </summary>
        [JsonProperty("hmd")]
        public SixDofPosition Hmd { get; set; }

        /// <summary>
        /// The player's ID.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// The player's left hand position.
        /// </summary>
        [JsonProperty("left")]
        public SixDofPosition Left { get; set; }

        /// <summary>
        /// The player's name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The player's right hand position.
        /// </summary>
        [JsonProperty("right")]
        public SixDofPosition Right { get; set; }
    }

    /// <summary>
    /// Sends the current ownership of the pendant.
    /// </summary>
    public partial class UnityPendantOut
    {
        /// <summary>
        /// The player's ID of the owner of the pendant.
        /// </summary>
        [JsonProperty("owner")]
        public string Owner { get; set; }
    }
}
