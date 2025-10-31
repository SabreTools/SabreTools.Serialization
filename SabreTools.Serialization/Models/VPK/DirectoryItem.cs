namespace SabreTools.Data.Models.VPK
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/VPKFile.h"/>
    /// <see href="https://developer.valvesoftware.com/wiki/VPK_(file_format)"/>
    public sealed class DirectoryItem
    {
        public string? Extension { get; set; }

        public string? Path { get; set; }

        public string? Name { get; set; }

        public DirectoryEntry? DirectoryEntry { get; set; }

        public byte[] PreloadData { get; set; }
    }
}
