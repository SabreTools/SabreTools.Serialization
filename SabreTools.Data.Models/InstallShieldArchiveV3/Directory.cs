using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.InstallShieldArchiveV3
{
    /// <see href="https://github.com/wfr/unshieldv3/blob/master/ISArchiveV3.cpp"/>
    [StructLayout(LayoutKind.Sequential)]
    public class Directory
    {
        /// <summary>
        /// Number of files in the directory
        /// </summary>
        public ushort FileCount;

        /// <summary>
        /// Size of the chunk
        /// </summary>
        public ushort ChunkSize;

        /// <summary>
        /// Length of the following ASCII string
        /// </summary>
        public ushort NameLength;

        /// <summary>
        /// Name as an ASCII string
        /// </summary>
        public string Name = string.Empty;
    }
}
