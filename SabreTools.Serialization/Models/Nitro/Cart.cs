namespace SabreTools.Data.Models.Nitro
{
    /// <summary>
    /// Represents a DS/DSi cart image
    /// </summary>
    public class Cart
    {
        /// <summary>
        /// DS/DSi cart header
        /// </summary>
        public CommonHeader CommonHeader { get; set; }

        /// <summary>
        /// DSi extended cart header
        /// </summary>
        public ExtendedDSiHeader ExtendedDSiHeader { get; set; }

        /// <summary>
        /// Secure area, may be encrypted or decrypted
        /// </summary>
        /// <remarks>0x800 bytes</remarks>
        public byte[] SecureArea { get; set; } = new byte[0x800];

        /// <summary>
        /// Name table (folder allocation table, name list)
        /// </summary>
        public NameTable NameTable { get; set; }

        /// <summary>
        /// File allocation table
        /// </summary>
        public FileAllocationTableEntry[] FileAllocationTable { get; set; }
    }
}
