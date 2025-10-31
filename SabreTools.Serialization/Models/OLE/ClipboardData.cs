namespace SabreTools.Data.Models.OLE
{
    /// <summary>
    /// The ClipboardData packet represents clipboard data
    /// </summary>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/>
    public class ClipboardData
    {
        /// <summary>
        /// The total size in bytes of the <see cref="Format"> and <see cref="Data"> fields,
        /// not including padding (if any).
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// An application-specific identifier for the format of the data in the
        /// <see cref="Data"> field.
        /// </summary>
        public uint Format { get; set; }

        /// <summary>
        /// MUST be an array of bytes, followed by zero padding to a multiple of 4 bytes
        /// </summary>
        public byte[] Data { get; set; }
    }
}
