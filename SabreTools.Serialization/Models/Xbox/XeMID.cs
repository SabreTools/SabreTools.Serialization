namespace SabreTools.Serialization.Models.Xbox
{
    /// <summary>
    /// Contains information specific to an XGD disc
    /// </summary>
    /// <remarks>
    /// XGD2/3 XeMID Format Information:
    ///
    /// AABCCCDDEFFGHH(IIIIIIII)
    /// - AA        => The two-ASCII-character publisher identifier (see Constants.Publishers for details)
    /// - B         => Platform identifier; 2 indicates Xbox 360.
    /// - CCC       => Game ID
    /// - DD        => SKU number (unique per SKU of a title)
    /// - E         => Region identifier (see Constants.Regions for details)
    /// - FF        => Base version; usually starts at 01 (can be 1 or 2 characters)
    /// - G         => Media type identifier (see Constants.MediaSubtypes for details)
    /// - HH        => Disc number stored in [disc number][total discs] format
    /// - IIIIIIII  => 8-hex-digit certification submission identifier; usually on test discs only
    /// </remarks>
    public class XeMID
    {
        /// <summary>
        /// 2-character publisher identifier
        /// </summary>
        public string? PublisherIdentifier { get; set; }

        /// <summary>
        /// 1-character Platform disc is made for, 2 indicates Xbox 360
        /// </summary>
        public char PlatformIdentifier { get; set; }

        /// <summary>
        /// 3-character Game ID
        /// </summary>
        public string? GameID { get; set; }

        /// <summary>
        /// 2-character Title-specific SKU
        /// </summary>
        public string? SKU { get; set; }

        /// <summary>
        /// 1-character Region identifier character
        /// </summary>
        public char RegionIdentifier { get; set; }

        /// <summary>
        /// 2-character Base version of executables, usually starts at 01
        /// </summary>
        public string? BaseVersion { get; set; }

        /// <summary>
        /// 1-character Media subtype identifier
        /// </summary>
        public char MediaSubtypeIdentifier { get; set; }

        /// <summary>
        /// 2-character Disc number stored in [disc number][total discs] format
        /// </summary>
        public string? DiscNumberIdentifier { get; set; }

        /// <summary>
        /// 8-hex-digit certification submission identifier; usually on test discs only
        /// </summary>
        public string? CertificationSubmissionIdentifier { get; set; }
    }
}
