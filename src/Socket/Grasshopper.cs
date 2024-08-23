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
}
