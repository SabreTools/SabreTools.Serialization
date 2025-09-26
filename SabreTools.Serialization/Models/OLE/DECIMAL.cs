namespace SabreTools.Data.Models.OLE
{
    /// <summary>
    /// The DECIMAL (Packet Version) packet represents a DECIMAL as specified in [MS-OAUT] section
    /// 2.2.26
    /// </summary>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/> 
    public class DECIMAL
    {
        /// <summary>
        /// MUST be set to zero and MUST be ignored
        /// </summary>
        public ushort Reserved { get; set; }

        /// <summary>
        /// The value of the scale field specified in [MS-OAUT] section 2.2.26
        /// </summary>
        public byte Scale { get; set; }

        /// <summary>
        /// The value of the sign field specified in [MS-OAUT] section 2.2.26
        /// </summary>
        public byte Sign { get; set; }

        /// <summary>
        /// The value of the Hi32 field specified in [MS-OAUT] section 2.2.26
        /// </summary>
        public uint Hi32 { get; set; }

        /// <summary>
        /// The value of the Lo64 field specified in [MS-OAUT] section 2.2.26
        /// </summary>
        public ulong Lo64 { get; set; }
    }
}
