namespace SabreTools.Data.Models.SecuROM
{
    /// <remarks>
    /// Most DFA-protected files seem to also have additional encryption,
    /// possibly SecuROM DFE. Only early RC-encrypted executables can be
    /// parsed beyond the initial header.
    /// </remarks>
    public class DFAFile
    {
        /// <summary>
        /// "SDFA" 0x04 0x00 0x00 0x00
        /// </summary>
        /// <remarks>8 bytes</remarks>
        public byte[] Signature { get; set; } = new byte[8];

        /// <summary>
        /// Unknown value, possibly a block or header size
        /// </summary>
        /// <remarks>Only a value of 0x400 has been found</remarks>
        public uint BlockOrHeaderSize { get; set; }

        /// <summary>
        /// All entries in the file
        /// </summary>
        public DFAEntry[] Entries { get; set; }
    }
}
