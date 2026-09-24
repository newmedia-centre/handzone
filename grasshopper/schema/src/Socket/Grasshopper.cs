namespace Schema.Socket.Grasshopper
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Sends a json payload of the program to Unity to be deserialized as a program.
    /// </summary>
    public partial class GrasshopperProgramIn
    {
        /// <summary>
        /// If true, program is base64(gzip(json))
        /// </summary>
        [JsonProperty("compressed", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Compressed { get; set; }

        /// <summary>
        /// The string payload of the IProgram object to send to the server.
        /// </summary>
        [JsonProperty("program", NullValueHandling = NullValueHandling.Ignore)]
        public string Program { get; set; }

        /// <summary>
        /// Minimal joint-space waypoints (radians). If provided, clients may ignore 'program'.
        /// </summary>
        [JsonProperty("joints", NullValueHandling = NullValueHandling.Ignore)]
        public List<List<double>> Joints { get; set; }

    /// <summary>
    /// Optional per-waypoint durations in seconds. If length == joints.Length-1, each value applies between i->i+1.
    /// If length == joints.Length, values apply per frame; last value may be ignored.
    /// </summary>
    [JsonProperty("times", NullValueHandling = NullValueHandling.Ignore)]
    public List<double> Times { get; set; }

        /// <summary>
        /// Optional timestep between joint waypoints in seconds.
        /// </summary>
        [JsonProperty("dt", NullValueHandling = NullValueHandling.Ignore)]
        public double? Dt { get; set; }

        /// <summary>
        /// Units for joints (default 'rad').
        /// </summary>
        [JsonProperty("units", NullValueHandling = NullValueHandling.Ignore)]
        public string Units { get; set; }

        /// <summary>
        /// If true, consumers should treat this as a hard reload of the current program
        /// </summary>
        [JsonProperty("reload", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Reload { get; set; }

        /// <summary>
        /// A monotonically changing identifier to force cache invalidation on consumers (e.g., ISO
        /// timestamp or GUID)
        /// </summary>
        [JsonProperty("revision", NullValueHandling = NullValueHandling.Ignore)]
        public string Revision { get; set; }
    }

    /// <summary>
    /// Receives a json payload of the program from Unity to be deserialized as a program.
    /// </summary>
    public partial class GrasshopperProgramOut
    {
        /// <summary>
        /// The string payload of the IProgram object received from the server.
        /// </summary>
        [JsonProperty("program", NullValueHandling = NullValueHandling.Ignore)]
        public string Program { get; set; }

        /// <summary>
        /// Minimal joint-space waypoints (radians) broadcast to clients.
        /// </summary>
        [JsonProperty("joints", NullValueHandling = NullValueHandling.Ignore)]
        public List<List<double>> Joints { get; set; }

    /// <summary>
    /// Optional per-waypoint durations in seconds. If length == joints.Length-1, each value applies between i->i+1.
    /// If length == joints.Length, values apply per frame; last value may be ignored.
    /// </summary>
    [JsonProperty("times", NullValueHandling = NullValueHandling.Ignore)]
    public List<double> Times { get; set; }

        /// <summary>
        /// Optional timestep between joint waypoints in seconds.
        /// </summary>
        [JsonProperty("dt", NullValueHandling = NullValueHandling.Ignore)]
        public double? Dt { get; set; }

        /// <summary>
        /// Units for joints (default 'rad').
        /// </summary>
        [JsonProperty("units", NullValueHandling = NullValueHandling.Ignore)]
        public string Units { get; set; }

        /// <summary>
        /// If true, consumers should treat this as a hard reload of the current program
        /// </summary>
        [JsonProperty("reload", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Reload { get; set; }

        /// <summary>
        /// A monotonically changing identifier to force cache invalidation on consumers
        /// </summary>
        [JsonProperty("revision", NullValueHandling = NullValueHandling.Ignore)]
        public string Revision { get; set; }
    }

    /// <summary>
    /// Sends an instruction to run the program on the robot.
    /// </summary>
    public partial class GrasshopperRunIn
    {
        /// <summary>
        /// Whether the simulation should play or stop
        /// </summary>
        [JsonProperty("run")]
        public bool Run { get; set; }
    }

    /// <summary>
    /// Sends a json payload of the meshes to Unity.
    /// </summary>
    public partial class GrasshopperMeshesIn
    {
        [JsonProperty("mesh1", NullValueHandling = NullValueHandling.Ignore)]
        public MeshData Mesh1 { get; set; }

        [JsonProperty("mesh2", NullValueHandling = NullValueHandling.Ignore)]
        public MeshData Mesh2 { get; set; }

        [JsonProperty("mesh3", NullValueHandling = NullValueHandling.Ignore)]
        public MeshData Mesh3 { get; set; }

        [JsonProperty("mesh4", NullValueHandling = NullValueHandling.Ignore)]
        public MeshData Mesh4 { get; set; }

        [JsonProperty("mesh5", NullValueHandling = NullValueHandling.Ignore)]
        public MeshData Mesh5 { get; set; }
    }

    public partial class MeshData
    {
        [JsonProperty("tris")]
        public List<List<double>> Tris { get; set; }

        [JsonProperty("vertices")]
        public List<Vector3D> Vertices { get; set; }
    }

    /// <summary>
    /// Represents a 3D vector consisting of three components
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
}
