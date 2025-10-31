using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.InstallShieldCabinet
{
    /// <see href="https://github.com/twogood/unshield/blob/main/lib/cabfile.h"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class Descriptor
    {
        /// <summary>
        /// Offset to the descriptor strings
        /// </summary>
        public uint StringsOffset;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Reserved0;

        /// <summary>
        /// Offset to the component list
        /// </summary>
        public uint ComponentListOffset;

        /// <summary>
        /// Offset to the file table
        /// </summary>
        public uint FileTableOffset;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Reserved1;

        /// <summary>
        /// Size of the file table
        /// </summary>
        public uint FileTableSize;

        /// <summary>
        /// Redundant size of the file table
        /// </summary>
        public uint FileTableSize2;

        /// <summary>
        /// Number of directories
        /// </summary>
        public ushort DirectoryCount;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Reserved2;

        /// <summary>
        /// Reserved
        /// </summary>
        public ushort Reserved3;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Reserved4;

        /// <summary>
        /// Number of files
        /// </summary>
        public uint FileCount;

        /// <summary>
        /// Redundant offset to the file table
        /// </summary>
        public uint FileTableOffset2;

        /// <summary>
        /// Number of component table infos
        /// </summary>
        public ushort ComponentTableInfoCount;

        /// <summary>
        /// Offset to the component table
        /// </summary>
        public uint ComponentTableOffset;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Reserved5;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Reserved6;

        /// <summary>
        /// Offsets to the file groups
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 71)]
        public uint[] FileGroupOffsets;

        /// <summary>
        /// Offsets to the components
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 71)]
        public uint[] ComponentOffsets;

        /// <summary>
        /// Offset to the setup types
        /// </summary>
        public uint SetupTypesOffset;

        /// <summary>
        /// Offset to the setup table
        /// </summary>
        public uint SetupTableOffset;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Reserved7;

        /// <summary>
        /// Reserved
        /// </summary>
        public uint Reserved8;
    }
}
