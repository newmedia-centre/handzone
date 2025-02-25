namespace Schema.Socket.Index
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Information about all the robot sessions currently available
    /// </summary>
    public partial class SessionsOut
    {
        /// <summary>
        /// the capacity for new virtual robots
        /// </summary>
        [JsonProperty("capacity")]
        public double Capacity { get; set; }

        /// <summary>
        /// The available virtual robot sessions
        /// </summary>
        [JsonProperty("sessions")]
        public List<RobotSession> Sessions { get; set; }
    }

    /// <summary>
    /// Information about a robot session
    /// </summary>
    public partial class RobotSession
    {
        /// <summary>
        /// The address of the robot
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// The name of the robot
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The session type
        /// </summary>
        [JsonProperty("type")]
        public SessionTypeEnum Type { get; set; }

        /// <summary>
        /// The names of users in the session
        /// </summary>
        [JsonProperty("users")]
        public List<string> Users { get; set; }
    }

    /// <summary>
    /// Join Session Payload.
    /// </summary>
    public partial class JoinSessionOut
    {
        /// <summary>
        /// The address of the socket namespace this session runs on.
        /// </summary>
        [JsonProperty("robot")]
        public RobotInfo Robot { get; set; }

        /// <summary>
        /// The token needed to join the session.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; set; }
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
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// The name of the robot
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    /// <summary>
    /// The session type
    /// </summary>
    public enum SessionTypeEnum { Exercises, Sandbox };

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                SessionTypeEnumConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class SessionTypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(SessionTypeEnum) || t == typeof(SessionTypeEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "exercises":
                    return SessionTypeEnum.Exercises;
                case "sandbox":
                    return SessionTypeEnum.Sandbox;
            }
            throw new Exception("Cannot unmarshal type SessionTypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (SessionTypeEnum)untypedValue;
            switch (value)
            {
                case SessionTypeEnum.Exercises:
                    serializer.Serialize(writer, "exercises");
                    return;
                case SessionTypeEnum.Sandbox:
                    serializer.Serialize(writer, "sandbox");
                    return;
            }
            throw new Exception("Cannot marshal type SessionTypeEnum");
        }

        public static readonly SessionTypeEnumConverter Singleton = new SessionTypeEnumConverter();
    }
}
