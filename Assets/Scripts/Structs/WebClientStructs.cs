using Schema.Socket;

namespace XRZone
{
    /// <summary>
    /// This is data that is send to the server.
    /// </summary>
    public struct PositionDataIn
    {
        public Vector6D hmd;
        public Vector6D left;
        public Vector6D right;
    }

    /// <summary>
    /// This is data that is received from the server
    /// </summary>
    public struct PositionDataOut
    {
        public string id;
        public Vector6D hmd;
        public Vector6D left;
        public Vector6D right;
        public string name;
        public string color;
    }

    public struct PendantDataOut
    {
        public string owner;
        public Vector6D position;
    }
}