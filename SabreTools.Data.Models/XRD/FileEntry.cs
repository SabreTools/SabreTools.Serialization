namespace SabreTools.Data.Models.XRD
{
    public class FileEntry
    {
        /// <summary>
        /// Sector offset of file in XDVDFS filesystem
        /// </summary>
        public uint Offset { get; set; }

        /// <summary>
        /// Size of file in bytes
        /// </summary>
        public ulong Size { get; set; }

        /// <summary>
        /// SHA-1 Hash of file content
        /// </summary>
        public byte[] SHA1 { get; set; } = new byte[20];
    }
}
