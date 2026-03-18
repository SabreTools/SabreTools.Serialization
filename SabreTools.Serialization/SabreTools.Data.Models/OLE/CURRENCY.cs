namespace SabreTools.Data.Models.OLE
{
    /// <summary>
    /// The CURRENCY (Packet Version) packet represents a CURRENCY as specified in [MS-OAUT]
    /// section 2.2.24.
    /// </summary>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/>
    public class CURRENCY
    {
        /// <summary>
        /// The value of the int64 field specified in [MS-OAUT] section 2.2.24.
        /// </summary>
        public ulong Value { get; set; }
    }
}
