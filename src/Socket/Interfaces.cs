namespace Schema.Socket.Interfaces
{

    /// <summary>
    /// Set tool digital output signal level.
    /// </summary>
    public partial class InterfacesSetToolDigitalOutIn
    {
        /// <summary>
        /// The signal level (boolean).
        /// </summary>
        public bool B { get; set; }

        /// <summary>
        /// The number (id) of the output, integer: [0:1].
        /// </summary>
        public double N { get; set; }
    }

    /// <summary>
    /// Set standard digital output signal level.
    /// </summary>
    public partial class InterfacesSetStandardDigitalOutIn
    {
        /// <summary>
        /// The signal level (boolean).
        /// </summary>
        public bool B { get; set; }

        /// <summary>
        /// The number (id) of the output, integer: [0:7].
        /// </summary>
        public double N { get; set; }
    }
}
