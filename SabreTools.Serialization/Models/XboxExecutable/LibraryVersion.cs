namespace SabreTools.Data.Models.XboxExecutable
{
    /// <summary>
    /// XBox Executable library version
    /// </summary>
    /// <see href="https://www.caustik.com/cxbx/download/xbe.htm"/>
    public class LibraryVersion
    {
        /// <summary>
        /// 8-byte name of this library. (i.e. "XAPILIB")
        /// </summary>
        public byte[] LibraryName { get; set; } = new byte[8];

        /// <summary>
        /// Major version for this library (2-byte WORD).
        /// </summary>
        public ushort MajorVersion { get; set; }

        /// <summary>
        /// Minor version for this library (2-byte WORD).
        /// </summary>
        public ushort MinorVersion { get; set; }

        /// <summary>
        /// Build version for this library (2-byte WORD).
        /// </summary>
        public ushort BuildVersion { get; set; }

        /// <summary>
        /// Various flags for this library.
        /// </summary>
        public LibraryFlags LibraryFlags { get; set; }
    }
}
