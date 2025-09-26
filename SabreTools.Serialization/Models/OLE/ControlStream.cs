namespace SabreTools.Data.Models.OLE
{
    /// <summary>
    /// A file that has one or more property sets associated with it through the alternate
    /// stream binding MUST have a control stream, which is an alternate stream with the name
    /// "{4c8cc155-6c1e-11d1-8e41-00c04fb9386d}". This stream MUST contain the following packet.
    /// </summary>
    /// <see href="https://winprotocoldoc.z19.web.core.windows.net/MS-OLEPS/%5bMS-OLEPS%5d.pdf"/> 
    public class ControlStream
    {
        /// <summary>
        /// MUST be set to zero, and nonzero values MUST be rejected.
        /// </summary>
        public ushort Reserved1 { get; set; }

        /// <summary>
        /// MUST be set to zero, and MUST be ignored.
        /// </summary>
        public ushort Reserved2 { get; set; }

        /// <summary>
        /// An application-provided value that MUST NOT be interpreted by the
        /// OLEPS implementation. If the application did not provide a value,
        /// it SHOULD be set to zero.
        /// </summary>
        public uint ApplicationState { get; set; }

        /// <summary>
        /// An application-provided value that MUST NOT be interpreted by the
        /// OLEPS implementation. If the application did not provide a value,
        /// it SHOULD be absent.
        /// </summary>
        public GUID? CLSID { get; set; }
    }
}
