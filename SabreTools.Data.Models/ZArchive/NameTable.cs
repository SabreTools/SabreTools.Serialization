namespace SabreTools.Data.Models.ZArchive
{
    /// <summary>
    /// UTF-8 strings, prepended by string lengths
    /// </summary>
    /// <see href="https://github.com/Exzap/ZArchive/"/>
    public class NameTable
    {
        /// <summary>
        /// List of filename entries
        /// </summary>
        public NameEntry[] NameEntries { get; set; } = [];

        /// <summary>
        /// Virtual field, to cache the offsets of each name entry in the name table
        /// Used for referencing the name entry from an offset into the name table
        /// </summary>
        public uint[] NameTableOffsets { get; set; } = [];
    }
}
