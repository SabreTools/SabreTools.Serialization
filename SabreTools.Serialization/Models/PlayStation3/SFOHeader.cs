using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.PlayStation3
{
    /// <see href="https://psdevwiki.com/ps3/PARAM.SFO"/> 
    [StructLayout(LayoutKind.Sequential)]
    public class SFOHeader
    {
        /// <summary>
        /// "\0PSF"
        /// </summary>
        public uint Magic;

        /// <summary>
        /// Version
        /// </summary>
        public uint Version;

        /// <summary>
        /// Absolute start offset of key_table
        /// </summary>
        public uint KeyTableStart;

        /// <summary>
        /// Absolute start offset of data_table
        /// </summary>
        public uint DataTableStart;

        /// <summary>
        /// Number of entries in index_table, key_table, and data_table
        /// </summary>
        public uint TablesEntries;
    }
}
