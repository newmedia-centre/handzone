namespace Schema.Socket.NDI
{

    public partial class VideoFrame
    {
        public string Data { get; set; }
        public double FourCc { get; set; }
        public double FrameFormatType { get; set; }
        public double FrameRateD { get; set; }
        public double FrameRateN { get; set; }
        public double LineStrideBytes { get; set; }
        public double PictureAspectRatio { get; set; }
        public double[] Timecode { get; set; }
        public double[] Timestamp { get; set; }
        public TypeEnum Type { get; set; }
        public double Xres { get; set; }
        public double Yres { get; set; }
    }

    public enum TypeEnum { Video };
}
