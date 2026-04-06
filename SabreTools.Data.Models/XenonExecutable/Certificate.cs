namespace SabreTools.Data.Models.XenonExecutable
{
    /// <summary>
    /// Xenon (Xbox 360) Executable format certificate structure
    /// </summary>
    /// <see href="http://oskarsapps.mine.nu/xexdump"/>
    /// <see href="https://free60.org/System-Software/Formats/XEX/"/>
    public class Certificate
    {
        /// <summary>
        /// Length of the certificate structure in bytes
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint Length { get; set; }

        /// <summary>
        /// Size of the entire image in bytes
        /// Uncompressed size (not the raw XEX file size)
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint ImageSize { get; set; }

        /// <summary>
        /// Public signature for XEX file
        /// Signed by Microsoft for XEX authenticity
        /// </summary>
        public byte[] Signature { get; set; } = new byte[256];

        /// <summary>
        /// Base File Load Address, retail discs always(?) 0x00000174
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint BaseFileLoadAddress { get; set; }

        /// <summary>
        /// Image flags, see Constants.ImageFlags
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint ImageFlags { get; set; }

        /// <summary>
        /// Base address for the image to be placed in memory
        /// Known values: 0x82000000 (retail games), 0x92000000 (applications)
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint ImageBaseAddress { get; set; }

        /// <summary>
        /// Unknown hash, likely SHA-1 integrity hash for portion of image
        /// </summary>
        public byte[] UnknownHash1 { get; set; } = new byte[20];

        /// <summary>
        /// Unknown field
        /// Known values: 0x00000002 (retail games, some applications), 0x00000003 (applications)
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint Unknown0128 { get; set; }

        /// <summary>
        /// Unknown hash, likely SHA-1 integrity hash for portion of image
        /// </summary>
        public byte[] UnknownHash2 { get; set; } = new byte[20];

        /// <summary>
        /// Full Media ID, probably a GUID
        /// Last (LSB) four bytes in hexadecimal are the unique ringcode of the disc being pressed
        /// </summary>
        public byte[] MediaID { get; set; } = new byte[16];

        /// <summary>
        /// XEX File Key
        /// </summary>
        public byte[] XEXFileKey { get; set; } = new byte[16];

        /// <summary>
        /// Unknown field, often zeroed
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint Unknown0160 { get; set; }

        /// <summary>
        /// Unknown hash, likely SHA-1 integrity hash for portion of image
        /// </summary>
        public byte[] UnknownHash3 { get; set; } = new byte[20];

        /// <summary>
        /// Flags for console region locking, known values in Constants.RegionFlags
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint RegionFlags { get; set; }

        /// <summary>
        /// Allowed media type flags, see Constants.AllowedMediaTypeFlags
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint AllowedMediaTypeFlags { get; set; }

        /// <summary>
        /// Number of entries in the following table
        /// </summary>
        /// <remarks>Big-endian</remarks>
        public uint TableCount { get; set; }

        /// <summary>
        /// Table, 24-bytes per entry
        /// </summary>
        public TableEntry[] Table { get; set; } = [];
    }
}
