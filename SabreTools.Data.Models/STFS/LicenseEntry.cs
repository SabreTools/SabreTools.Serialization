using System.Collections.Generic;

namespace SabreTools.Data.Models.STFS
{
    /// <summary>
    /// Secure Transacted File System (STFS) License Entry format
    /// </summary>
    /// <see href="https://free60.org/System-Software/Formats/STFS/"/>
    public class LicenseEntry
    {
        /// <summary>
        /// License ID
        /// </summary>
        public long LicenseID { get; set; }

        /// <summary>
        /// License Bits
        /// </summary>
        public int LicenseBits { get; set; }

        /// <summary>
        /// License Flags
        /// </summary>
        public int LicenseFlags { get; set; }
    }
}
