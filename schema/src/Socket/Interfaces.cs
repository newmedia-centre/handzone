namespace Schema.Socket.Interfaces
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Set tool digital output signal level.
    /// </summary>
    public partial class InterfacesSetToolDigitalOutIn
    {
        /// <summary>
        /// The signal level (boolean).
        /// </summary>
        [JsonProperty("b")]
        public bool B { get; set; }

        /// <summary>
        /// The number (id) of the output, integer: [0:1].
        /// </summary>
        [JsonProperty("n")]
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
        [JsonProperty("b")]
        public bool B { get; set; }

        /// <summary>
        /// The number (id) of the output, integer: [0:7].
        /// </summary>
        [JsonProperty("n")]
        public double N { get; set; }
    }
}
