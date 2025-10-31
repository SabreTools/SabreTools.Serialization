namespace SabreTools.Data.Models.OLE
{
    /// <summary>
    /// The VersionedStream packet represents a stream with an application-specific version GUID.
    /// </summary>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/>
    public class VersionedStream
    {
        /// <summary>
        /// MUST be a GUID (Packet Version).
        /// </summary>
        public GUID VersionGuid { get; set; }

        /// <summary>
        /// MUST be an IndirectPropertyName.
        /// </summary>
        public IndirectPropertyName StreamName { get; set; }
    }
}
