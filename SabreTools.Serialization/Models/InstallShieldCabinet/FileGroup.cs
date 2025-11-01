namespace SabreTools.Data.Models.InstallShieldCabinet
{
    /// <see href="https://github.com/twogood/unshield/blob/main/lib/libunshield.h"/>
    /// <remarks>Additional info from i6comp02</remarks>
    public sealed class FileGroup
    {
        /// <summary>
        /// Offset to the file group name
        /// </summary>
        public uint NameOffset { get; set; }

        /// <summary>
        /// File group name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Size of the expanded data
        /// </summary>
        public uint ExpandedSize { get; set; }

        /// <summary>
        /// Size of the compressed data
        /// </summary>
        public uint CompressedSize { get; set; }

        /// <summary>
        /// Attributes (junk2)
        /// </summary>
        public FileGroupAttributes Attributes { get; set; }

        /// <summary>
        /// Index of the first file
        /// </summary>
        public uint FirstFile { get; set; }

        /// <summary>
        /// Index of the last file
        /// </summary>
        public uint LastFile { get; set; }

        /// <summary>
        /// Unknown string offset
        /// </summary>
        public uint UnknownStringOffset { get; set; }

        /// <summary>
        /// Offset to the operating system (Var4)
        /// </summary>
        public uint OperatingSystemOffset { get; set; }

        /// <summary>
        /// Offset to the language (Var1)
        /// </summary>
        public uint LanguageOffset { get; set; }

        /// <summary>
        /// Offset to the HTTP location
        /// </summary>
        public uint HTTPLocationOffset { get; set; }

        /// <summary>
        /// Offset to the FTP location
        /// </summary>
        public uint FTPLocationOffset { get; set; }

        /// <summary>
        /// Misc offset
        /// </summary>
        public uint MiscOffset { get; set; }

        /// <summary>
        /// Offset to the target directory
        /// </summary>
        public uint TargetDirectoryOffset { get; set; }

        /// <summary>
        /// Overwrite setting flags
        /// </summary>
        public FileGroupFlags OverwriteFlags { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public uint[] Reserved { get; set; } = new uint[4];
    }
}
