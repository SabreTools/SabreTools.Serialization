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
        public uint InstallerBaseVersion { get; set; }

        /// <summary>
        /// Field for version number number, major.minor.build.revision
        /// </summary>
        /// <remarks>4 bytes</remarks>
        public uint InstallerVersion { get; set; }
    }
}
