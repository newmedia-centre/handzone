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
        /// The string payload of the IProgram object to send to the server.
        /// </summary>
        [JsonProperty("program")]
        public string Program { get; set; }
    }

    /// <summary>
    /// Receives a json payload of the program from Unity to be deserialized as a program.
    /// </summary>
    public partial class GrasshopperProgramOut
    {
        /// <summary>
        /// The string payload of the IProgram object received from the server.
        /// </summary>
        [JsonProperty("program")]
        public string Program { get; set; }
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
        [JsonProperty("mesh1")]
        public MeshData Mesh1 { get; set; }

        [JsonProperty("mesh2")]
        public MeshData Mesh2 { get; set; }

        [JsonProperty("mesh3")]
        public MeshData Mesh3 { get; set; }

        [JsonProperty("mesh4")]
        public MeshData Mesh4 { get; set; }

        [JsonProperty("mesh5")]
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
