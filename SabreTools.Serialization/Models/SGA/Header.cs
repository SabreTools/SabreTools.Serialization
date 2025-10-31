namespace SabreTools.Data.Models.SGA
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public abstract class Header
    {
        public string Signature { get; set; } = string.Empty;

        public ushort MajorVersion { get; set; }

        public ushort MinorVersion { get; set; }
    }
}
