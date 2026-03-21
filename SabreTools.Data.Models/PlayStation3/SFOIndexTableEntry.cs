using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.PlayStation3
{
    /// <see href="https://psdevwiki.com/ps3/PARAM.SFO"/>
    [StructLayout(LayoutKind.Sequential)]
    public class SFOIndexTableEntry
    {
        /// <summary>
        /// Key relative offset.
        /// (Absolute start offset of key) - (Absolute start offset of key_table)
        /// </summary>
        public ushort KeyOffset;

        /// <summary>
        /// Data type
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public DataFormat DataFormat;

        /// <summary>
        /// Data used length
        /// </summary>
        public uint DataLength;

        /// <summary>
        /// Data total length. TITLE_ID is always = 16 bytes
        /// </summary>
        public uint DataMaxLength;

        /// <summary>
        /// Data relative offset.
        /// (Absolute start offset of data_1) - (Absolute start offset of data_table)
        /// </summary>
        public uint DataOffset;
    }
}
