namespace SabreTools.Data.Models.STFS
{
    /// <summary>
    /// Signature block signed by Microsoft, for LIVE and PIRS format STFS files
    /// </summary>
    /// <see href="https://free60.org/System-Software/Formats/STFS/"/>
    public class MicrosoftSignature : Signature
    {
        /// <summary>
        /// Signature remotely signed by Microsoft
        /// </summary>
        /// <remarks>256 bytes</remarks>
        public byte[] PackageSignature { get; set; } = new byte[256];

        /// <summary>
        /// Zeroed padding
        /// </summary>
        /// <remarks>296 bytes</remarks>
        public byte[] Padding { get; set; } = new byte[296];
    }
}
