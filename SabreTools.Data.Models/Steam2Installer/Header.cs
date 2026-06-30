using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.Steam2Installer
{
    /// <summary>
    /// Header from the .sim file.
    /// </summary>
    public sealed class Header
    {
        /// <summary>
        /// .sim file magic value
        /// </summary>
        public uint Magic { get; set; }

        /// <summary>
        /// Version (0x00000001)
        /// </summary>
        public uint Version { get; set; }

        /// <summary>
        /// Number of discs
        /// </summary>
        public uint Discs { get; set; }

        /// <summary>
        /// Size of the section containing file and path strings.
        /// </summary>
        public uint StringsSize { get; set; }
    }
}
