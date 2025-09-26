namespace SabreTools.Serialization.Models.OLE
{
    /// <summary>
    /// The GUID (Packet Version) packet represents a GUID
    /// </summary>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/> 
    public class GUID
    {
        /// <summary>
        /// The value of the Data1 field specified in [MS-DTYP] section 2.3.4
        /// </summary>
        public uint Data1 { get; set; }

        /// <summary>
        /// The value of the Data2 field specified in [MS-DTYP] section 2.3.4
        /// </summary>
        public ushort Data2 { get; set; }

        /// <summary>
        /// The value of the Data3 field specified in [MS-DTYP] section 2.3.4
        /// </summary>
        public ushort Data3 { get; set; }

        /// <summary>
        /// The value of the Data4 field specified in [MS-DTYP] section 2.3.4
        /// </summary>
        public byte[]? Data4 { get; set; }
    }
}
