namespace Schema.Socket.Grasshopper
{

    /// <summary>
    /// Sends a json payload of the program to Unity to be deserialized as a program.
    /// </summary>
    public partial class GrasshopperProgramIn
    {
        /// <summary>
        /// The the json payload of the IProgram object to send to the server.
        /// </summary>
        public string Json { get; set; }
    }

    /// <summary>
    /// Sends a json payload of the program to Unity to be deserialized as a program.
    /// </summary>
    public partial class GrasshopperProgramOut
    {
        /// <summary>
        /// The json payload of the IProgram object received from the server.
        /// </summary>
        public string Json { get; set; }
    }
}
