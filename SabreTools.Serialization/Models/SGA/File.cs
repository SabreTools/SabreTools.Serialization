namespace SabreTools.Serialization.Models.SGA
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/SGAFile.h"/>
    public abstract class File
    {
        public uint NameOffset { get; set; }

        public string? Name { get; set; }

        public uint Offset { get; set; }

        public uint SizeOnDisk { get; set; }

        public uint Size { get; set; }

        public uint TimeModified { get; set; }

        public byte Dummy0 { get; set; }

        public byte Type { get; set; }
    }
}
