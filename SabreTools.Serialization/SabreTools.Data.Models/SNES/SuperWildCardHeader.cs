namespace SabreTools.Data.Models.SNES
{
    /// <summary>
    /// Super Wild Card (SWC) header
    /// </summary>
    /// <see href="https://www.raphnet.net/divers/documentation/Sneskart.txt"/>
    public class SuperWildCardHeader : Header
    {
        /// <summary>
        /// Size word
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public ushort Size { get; set; }

        /// <summary>
        /// Image information byte
        /// </summary>
        public SWCImageInformation ImageInformation { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        /// <remarks>4 bytes</remarks>
        public byte[] Reserved1 { get; set; } = new byte[4];

        /// <summary>
        /// SWC header identifier (0xAA, 0xBB, 0x04)
        /// </summary>
        public byte[] HeaderIdentifier { get; set; } = new byte[3];

        /// <summary>
        /// Padding to get to 512 bytes
        /// </summary>
        /// <remarks>502 bytes</remarks>
        public byte[] Padding { get; set; } = new byte[502];
    }
}
