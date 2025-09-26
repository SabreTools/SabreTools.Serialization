namespace SabreTools.Data.Models.OLE
{
    /// <summary>
    /// The FILETIME (Packet Version) packet represents a FILETIME structure ([MS-DTYP] section 2.3.3
    /// </summary>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/> 
    public class FILETIME
    {
        /// <summary>
        /// The value of the dwLowDateTime field specified in [MS-DTYP] section 2.3.3.
        /// </summary>
        public uint LowDateTime { get; set; }

        /// <summary>
        /// The value of the dwHighDateTime field specified in [MS-DTYP] section 2.3.3.
        /// </summary>
        public uint HighDateTime { get; set; }
    }
}
