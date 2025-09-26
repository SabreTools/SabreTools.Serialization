namespace SabreTools.Data.Models.XZP
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/XZPFile.h"/>
    public sealed class DirectoryItem
    {
        public uint FileNameCRC { get; set; }

        public uint NameOffset { get; set; }

        public string? Name { get; set; }

        public uint TimeCreated { get; set; }
    }
}
