namespace SabreTools.Data.Models.OLE
{
    /// <summary>
    /// The BLOB packet represents binary data
    /// </summary>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/>
    public class BLOB
    {
        /// <summary>
        /// The size in bytes of the <see cref="Bytes"> field, not including padding (if any)
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// MUST be an array of bytes, followed by zero padding to a multiple of 4 bytes.
        /// </summary>
        public byte[] Bytes { get; set; }
    }
}
