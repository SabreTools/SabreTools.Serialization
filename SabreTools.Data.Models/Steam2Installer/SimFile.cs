namespace SabreTools.Data.Models.Steam2Installer
{
    /// <summary>
    /// .sim file for a .sim/.sid archive
    /// </summary>
    /// <remarks>
    /// While it is not known for sure what .sim stands for, it's assumed to mean Steam Installer Manifest.
    /// This contains information about the .sid data volumes. It's used for retail installer discs for
    /// Steam2 (Steam pre-Steam3/SteamPipe). Preceded by GCF (GCF and NCF are still used for actual game
    /// installs on Steam2, sim/sid just replaced GCF for retail discs. NCF was never directly stored on
    /// retail discs since sim/sid had already come out). Succeeded by csm/csd for Steam3/SteamPipe.
    /// Code is based on NickNine's python-based sim/sid extractor at https://pastebin.com/sj6F64XP
    /// While this was not referenced for any of the code in SabreTools, https://codeberg.org/CYBERDEV/SIDEx
    /// provides additional documentation if it is needed.
    /// </remarks>
    public sealed class SimFile
    {
        /// <summary>
        /// .sim file header
        /// </summary>
        public Header Header { get; set; } = new();

        /// <summary>
        /// Byte array containing the null-terminated strings for file paths and names
        /// </summary>
        /// <remarks>Seems to usually start with two null bytes, not that it matters with how this gets parsed.</remarks>
        public byte[] StringsBytes { get; set; } = [];

        /// <summary>
        /// Size of the file entries section
        /// </summary>
        public uint FileEntriesSize { get; set; }

        /// <summary>
        /// Number of entries in the file entries section
        /// </summary>
        public uint FileEntriesCount { get; set; }

        /// <summary>
        /// Array of file entries
        /// </summary>
        public FileEntry[] FileEntries { get; set; } = [];
    }
}
