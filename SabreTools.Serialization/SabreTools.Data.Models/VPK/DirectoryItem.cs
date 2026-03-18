namespace SabreTools.Data.Models.VPK
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/VPKFile.h"/>
    /// <see href="https://developer.valvesoftware.com/wiki/VPK_(file_format)"/>
    public sealed class DirectoryItem
    {
        public string Extension { get; set; } = string.Empty;

        public string Path { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public DirectoryEntry DirectoryEntry { get; set; } = new();

        public byte[] PreloadData { get; set; } = [];
    }
}
