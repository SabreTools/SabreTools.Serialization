namespace SabreTools.Data.Models.XDVDFS
{
    /// <summary>
    /// Xbox DVD Layout Descriptor, present at sector 33 (offset 0x10800) of an Xbox DVD Filesystem
    /// Only present on XGD1 and XGD2 discs
    /// </summary>
    /// <see href="https://xboxdevwiki.net/XDVDFS"/>
    /// <see href="https://github.dev/Deterous/XboxKit/"/>
    public class LayoutDescriptor
    {
        /// <summary>
        /// Xbox DVD Layout descriptor signature for 2nd sector start
        /// For XGD2, this should be the only non-zero field
        /// </summary>
        /// <remarks>24 bytes</remarks>
        public byte[] Signature { get; set; } = new byte[24]; // LAYOUT_DESCRIPTOR_MAGIC

        /// <summary>
        /// Seemingly unused 8 bytes after the signature, should be zeroed
        /// </summary>
        /// <remarks>8 bytes</remarks>
        public byte[] Unusued8Bytes { get; set; } = new byte[8];

        /// <summary>
        /// Version number of xblayout tool used to master the filesystem
        /// Known versions are 1.0.x.1, x = 3926 to 5120
        /// If zeroed, xblayout was not used
        /// </summary>
        /// <remarks>8 bytes</remarks>
        public FourPartVersionType XBLayoutVersion { get; set; } = new();

        /// <summary>
        /// Version number of xbpremaster tool used to master the filesystem
        /// Often matches xblayout version
        /// If zeroed, xbpremaster was not used
        /// </summary>
        /// <remarks>8 bytes</remarks>
        public FourPartVersionType XBPremasterVersion { get; set; } = new();

        /// <summary>
        /// Version number of xbgamedisc tool used to master the filesystem
        /// The major version is set to 0x01 0x02 which may not be a ushort ?
        /// Known versions are 513.0.x.1 (aka 2.1.0.x.1), x = 5233 to 5849
        /// If zeroed, xbgamedisc was not used
        /// </summary>
        /// <remarks>8 bytes</remarks>
        public FourPartVersionType XBGameDiscVersion { get; set; } = new();

        /// <summary>
        /// Version number of other microsoft tool used to master the filesystem
        /// Always(?) matches xbgamedisc version
        /// May be zeroed, not always present
        /// </summary>
        /// <remarks>8 bytes</remarks>
        public FourPartVersionType XBOther1Version { get; set; } = new();

        /// <summary>
        /// Version number of other microsoft tool used to master the filesystem
        /// Always(?) matches xbgamedisc version
        /// May be zeroed, not always present
        /// </summary>
        /// <remarks>8 bytes</remarks>
        public FourPartVersionType XBOther2Version { get; set; } = new();

        /// <summary>
        /// Version number of other microsoft tool used to master the filesystem
        /// Always(?) matches xbgamedisc version
        /// May be zeroed, not always present
        /// </summary>
        /// <remarks>8 bytes</remarks>
        public FourPartVersionType XBOther3Version { get; set; } = new();

        /// <summary>
        /// Padding the remainder of sector, should be zeroed
        /// </summary>
        /// <remarks>1968 bytes</remarks>
        public byte[] Reserved { get; set; } = new byte[1968];
    }
}
