namespace SabreTools.Data.Models.AdvancedInstaller
{
    /// <summary>
    /// Single entry in the file table
    /// </summary>
    public class FileEntry
    {
        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>
        /// Observed values:
        /// - 00 00 00 00 (INI)
        /// - 01 00 00 00 (MSI, CAB)
        /// - 05 00 00 00 (DLL)
        /// </remarks>
        public uint Unknown0 { get; set; }

        /// <summary>
        /// Unknown
        /// </summary>
        /// <remarks>
        /// Observed values:
        /// - 00 00 00 00 (MSI)
        /// - 01 00 00 00 (CAB)
        /// - 03 00 00 00 (INI)
        /// - 0C 00 00 00 (DLL)
        /// </remarks>
        public uint? Unknown1 { get; set; }

        /// <summary>
        /// Unknown, always 0?
        /// </summary>
        /// <remarks>
        /// Observed values:
        /// - 00 00 00 00 (DLL, MSI, CAB, INI)
        /// </remarks>
        public uint? Unknown2 { get; set; }

        /// <summary>
        /// Size of the file
        /// </summary>
        public uint FileSize { get; set; }

        /// <summary>
        /// Offset of the file relative to the start
        /// of the SFX stub
        /// </summary>
        public uint FileOffset { get; set; }

        /// <summary>
        /// Size of the file name in characters
        /// </summary>
        public uint NameSize { get; set; }

        /// <summary>
        /// Unicode-encoded file name
        /// </summary>
        public string? Name { get; set; }
    }
}
