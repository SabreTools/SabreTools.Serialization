using SabreTools.Numerics;

namespace SabreTools.Data.Models.STFS
{
    /// <summary>
    /// STFS Volume Descriptor, for System or Title Update Installer STFS packages
    /// </summary>
    public class InstallerUpdateHeader : InstallerHeader
    {
        /// <summary>
        /// Field for base version number, major.minor.build.revision
        /// </summary>
        /// <remarks>4 bytes</remarks>
        public byte[] InstallerBaseVersion { get; set; } = new byte[4];

        /// <summary>
        /// Field for version number number, major.minor.build.revision
        /// </summary>
        /// <remarks>4 bytes</remarks>
        public byte[] InstallerVersion { get; set; } = new byte[4];
    }
}
