namespace SabreTools.Data.Models.OLE
{
    /// <summary>
    /// The DATE (Packet Version) packet represents a DATE as specified in [MS-OAUT]
    /// section 2.2.25
    /// </summary>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/>
    public class DATE
    {
        /// <summary>
        /// The value of the DATE is an 8-byte IEEE floating-point number, as specified in
        /// [MS-OAUT] section 2.2.25.
        /// </summary>
        public ulong Value { get; set; }
    }
}
