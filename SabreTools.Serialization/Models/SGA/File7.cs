namespace SabreTools.Serialization.Models.SGA
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public sealed class File7 : File
    {
        public uint CRC32 { get; set; }

        public uint HashOffset { get; set; }
    }
}
