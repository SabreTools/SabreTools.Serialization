namespace SabreTools.Data.Models.SGA
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public sealed class FileHeader
    {
        public string Name { get; set; }

        public uint CRC32 { get; set; }
    }
}
