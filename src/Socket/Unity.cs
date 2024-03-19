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
    /// Represents a 3D vector consisting of three components
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
    /// Represents a 6 degrees of freedom position with position and rotation vectors.
    /// </summary>
    public partial class SixDofPosition
    {
        /// <summary>
        /// The position vector.
        /// </summary>
        public SixDofPositionPosition Position { get; set; }

        /// <summary>
        /// The rotation vector.
        /// </summary>
        public SixDofPositionRotation Rotation { get; set; }
    }

    /// <summary>
    /// The position vector.
    /// </summary>
    public partial class SixDofPositionPosition
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
    /// The rotation vector.
    /// </summary>
    public partial class SixDofPositionRotation
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
        public PlayerDataHmd Hmd { get; set; }

        /// <summary>
        /// The player's ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The player's left hand position.
        /// </summary>
        public PlayerDataLeft Left { get; set; }

        /// <summary>
        /// The player's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The player's right hand position.
        /// </summary>
        public PlayerDataRight Right { get; set; }
    }

    /// <summary>
    /// The player's head-mounted display position.
    /// </summary>
    public partial class PlayerDataHmd
    {
        /// <summary>
        /// The position vector.
        /// </summary>
        public PurplePosition Position { get; set; }

        /// <summary>
        /// The rotation vector.
        /// </summary>
        public PurpleRotation Rotation { get; set; }
    }

    /// <summary>
    /// The position vector.
    /// </summary>
    public partial class PurplePosition
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
    /// The rotation vector.
    /// </summary>
    public partial class PurpleRotation
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
    /// The player's left hand position.
    /// </summary>
    public partial class PlayerDataLeft
    {
        /// <summary>
        /// The position vector.
        /// </summary>
        public FluffyPosition Position { get; set; }

        /// <summary>
        /// The rotation vector.
        /// </summary>
        public FluffyRotation Rotation { get; set; }
    }

    /// <summary>
    /// The position vector.
    /// </summary>
    public partial class FluffyPosition
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
    /// The rotation vector.
    /// </summary>
    public partial class FluffyRotation
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
    /// The player's right hand position.
    /// </summary>
    public partial class PlayerDataRight
    {
        /// <summary>
        /// The position vector.
        /// </summary>
        public TentacledPosition Position { get; set; }

        /// <summary>
        /// The rotation vector.
        /// </summary>
        public TentacledRotation Rotation { get; set; }
    }

    /// <summary>
    /// The position vector.
    /// </summary>
    public partial class TentacledPosition
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
    /// The rotation vector.
    /// </summary>
    public partial class TentacledRotation
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
    /// Represents a message from Unity.
    /// </summary>
    public partial class UnityMessageIn
    {
        /// <summary>
        /// The message content.
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Represents a player's position in Unity.
    /// </summary>
    public partial class UnityPlayerIn
    {
        /// <summary>
        /// The player's head-mounted display position.
        /// </summary>
        public UnityPlayerInHmd Hmd { get; set; }

        /// <summary>
        /// The player's left hand position.
        /// </summary>
        public UnityPlayerInLeft Left { get; set; }

        /// <summary>
        /// The player's right hand position.
        /// </summary>
        public UnityPlayerInRight Right { get; set; }
    }

    /// <summary>
    /// The player's head-mounted display position.
    /// </summary>
    public partial class UnityPlayerInHmd
    {
        /// <summary>
        /// The position vector.
        /// </summary>
        public StickyPosition Position { get; set; }

        /// <summary>
        /// The rotation vector.
        /// </summary>
        public StickyRotation Rotation { get; set; }
    }

    /// <summary>
    /// The position vector.
    /// </summary>
    public partial class StickyPosition
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
    /// The rotation vector.
    /// </summary>
    public partial class StickyRotation
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
    /// The player's left hand position.
    /// </summary>
    public partial class UnityPlayerInLeft
    {
        /// <summary>
        /// The position vector.
        /// </summary>
        public IndigoPosition Position { get; set; }

        /// <summary>
        /// The rotation vector.
        /// </summary>
        public IndigoRotation Rotation { get; set; }
    }

    /// <summary>
    /// The position vector.
    /// </summary>
    public partial class IndigoPosition
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
    /// The rotation vector.
    /// </summary>
    public partial class IndigoRotation
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
    /// The player's right hand position.
    /// </summary>
    public partial class UnityPlayerInRight
    {
        /// <summary>
        /// The position vector.
        /// </summary>
        public IndecentPosition Position { get; set; }

        /// <summary>
        /// The rotation vector.
        /// </summary>
        public IndecentRotation Rotation { get; set; }
    }

    /// <summary>
    /// The position vector.
    /// </summary>
    public partial class IndecentPosition
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
    /// The rotation vector.
    /// </summary>
    public partial class IndecentRotation
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
    /// Represents a pendant's position in Unity.
    /// </summary>
    public partial class UnityPendantIn
    {
        /// <summary>
        /// The pendant's position.
        /// </summary>
        public UnityPendantInPosition Position { get; set; }

        /// <summary>
        /// The pendant's rotation.
        /// </summary>
        public UnityPendantInRotation Rotation { get; set; }
    }

    /// <summary>
    /// The pendant's position.
    /// </summary>
    public partial class UnityPendantInPosition
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
    /// The pendant's rotation.
    /// </summary>
    public partial class UnityPendantInRotation
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
    /// Represents a message to Unity.
    /// </summary>
    public partial class UnityMessageOut
    {
        /// <summary>
        /// The message content.
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// Represents the data of players in Unity.
    /// </summary>
    public partial class UnityPlayerOut
    {
        /// <summary>
        /// An array of player data.
        /// </summary>
        public Player[] Players { get; set; }
    }

    /// <summary>
    /// Represents the data of a player.
    /// </summary>
    public partial class Player
    {
        /// <summary>
        /// The player's color.
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// The player's head-mounted display position.
        /// </summary>
        public PlayerHmd Hmd { get; set; }

        /// <summary>
        /// The player's ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The player's left hand position.
        /// </summary>
        public PlayerLeft Left { get; set; }

        /// <summary>
        /// The player's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The player's right hand position.
        /// </summary>
        public PlayerRight Right { get; set; }
    }

    /// <summary>
    /// The player's head-mounted display position.
    /// </summary>
    public partial class PlayerHmd
    {
        /// <summary>
        /// The position vector.
        /// </summary>
        public HilariousPosition Position { get; set; }

        /// <summary>
        /// The rotation vector.
        /// </summary>
        public HilariousRotation Rotation { get; set; }
    }

    /// <summary>
    /// The position vector.
    /// </summary>
    public partial class HilariousPosition
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
    /// The rotation vector.
    /// </summary>
    public partial class HilariousRotation
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
    /// The player's left hand position.
    /// </summary>
    public partial class PlayerLeft
    {
        /// <summary>
        /// The position vector.
        /// </summary>
        public AmbitiousPosition Position { get; set; }

        /// <summary>
        /// The rotation vector.
        /// </summary>
        public AmbitiousRotation Rotation { get; set; }
    }

    /// <summary>
    /// The position vector.
    /// </summary>
    public partial class AmbitiousPosition
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
    /// The rotation vector.
    /// </summary>
    public partial class AmbitiousRotation
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
    /// The player's right hand position.
    /// </summary>
    public partial class PlayerRight
    {
        /// <summary>
        /// The position vector.
        /// </summary>
        public CunningPosition Position { get; set; }

        /// <summary>
        /// The rotation vector.
        /// </summary>
        public CunningRotation Rotation { get; set; }
    }

    /// <summary>
    /// The position vector.
    /// </summary>
    public partial class CunningPosition
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
    /// The rotation vector.
    /// </summary>
    public partial class CunningRotation
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
    /// Represents a pendant's position in Unity.
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
        public UnityPendantOutPosition Position { get; set; }

        /// <summary>
        /// The pendant's rotation.
        /// </summary>
        public UnityPendantOutRotation Rotation { get; set; }
    }

    /// <summary>
    /// The pendant's position.
    /// </summary>
    public partial class UnityPendantOutPosition
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
    /// The pendant's rotation.
    /// </summary>
    public partial class UnityPendantOutRotation
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
}
