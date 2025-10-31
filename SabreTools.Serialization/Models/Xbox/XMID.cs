namespace SabreTools.Data.Models.Xbox
{
    /// <summary>
    /// Contains information specific to an XGD disc
    /// </summary>
    /// <remarks>
    /// XGD1 XMID Format Information:
    ///
    /// AABBBCCD
    /// - AA        => The two-ASCII-character publisher identifier (see Constants.Publishers for details)
    /// - BBB       => Game ID
    /// - CC        => Version number
    /// - D         => Region identifier (see Constants.Regions for details)
    /// </remarks>
    public class XMID
    {
        /// <summary>
        /// 2-character publisher identifier
        /// </summary>
        public string PublisherIdentifier { get; set; } = string.Empty;

        /// <summary>
        /// 3-character Game ID
        /// </summary>
        public string GameID { get; set; } = string.Empty;

        /// <summary>
        /// 2-character Internal version number
        /// </summary>
        public string VersionNumber { get; set; } = string.Empty;

        /// <summary>
        /// 1-character Region identifier character
        /// </summary
        public char RegionIdentifier { get; set; }
    }
}
