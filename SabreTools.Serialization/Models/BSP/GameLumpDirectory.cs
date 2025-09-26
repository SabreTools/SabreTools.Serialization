using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// The Game lump (Lump 35) seems to be intended to be used for
    /// map data that is specific to a particular game using the Source
    /// engine, so that the file format can be extended without altering
    /// the previously defined format.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class GameLumpDirectory
    {
        /// <summary>
        /// Gamelump ID
        /// </summary>
        public int Id;

        /// <summary>
        /// Flags
        /// </summary>
        public ushort Flags;

        /// <summary>
        /// Gamelump version
        /// </summary>
        public ushort Version;

        /// <summary>
        /// Offset to this gamelump
        /// </summary>
        public int FileOffset;

        /// <summary>
        /// Length
        /// </summary>
        public int FileLength;
    }
}