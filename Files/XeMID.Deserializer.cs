using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class XeMID : IFileSerializer<Models.Xbox.XeMID>
    {
        /// <inheritdoc/>
        /// <remarks>This treats the input path like a parseable string</remarks>
#if NET48
        public Models.Xbox.XeMID Deserialize(string path)
#else
        public Models.Xbox.XeMID? Deserialize(string? path)
#endif
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            string xemid = path.TrimEnd('\0');
            if (string.IsNullOrWhiteSpace(xemid))
                return null;

            return ParseXeMID(xemid);
        }

        /// <summary>
        /// Parse an XGD2/3 XeMID string
        /// </summary>
        /// <param name="xemidString">XeMID string to attempt to parse</param>
        /// <returns>Filled XeMID on success, null on error</returns>
#if NET48
        private static Models.Xbox.XeMID ParseXeMID(string xemidString)
#else
        private static Models.Xbox.XeMID? ParseXeMID(string? xemidString)
#endif
        {
            if (xemidString == null)
                return null;
            if (!(xemidString.Length == 13 || xemidString.Length == 14 || xemidString.Length == 21 || xemidString.Length == 22))
                return null;

            var xemid = new Models.Xbox.XeMID();

            xemid.PublisherIdentifier = xemidString.Substring(0, 2);
            xemid.PlatformIdentifier = xemidString[2];
            if (xemid.PlatformIdentifier != '2')
                return null;

            xemid.GameID = xemidString.Substring(3, 3);
            xemid.SKU = xemidString.Substring(6, 2);
            xemid.RegionIdentifier = xemidString[8];

            if (xemidString.Length == 13 || xemidString.Length == 21)
            {
                xemid.BaseVersion = xemidString.Substring(9, 1);
                xemid.MediaSubtypeIdentifier = xemidString[10];
                xemid.DiscNumberIdentifier = xemidString.Substring(11, 2);
            }
            else if (xemidString.Length == 14 || xemidString.Length == 22)
            {
                xemid.BaseVersion = xemidString.Substring(9, 2);
                xemid.MediaSubtypeIdentifier = xemidString[11];
                xemid.DiscNumberIdentifier = xemidString.Substring(12, 2);
            }

            if (xemidString.Length == 21)
                xemid.CertificationSubmissionIdentifier = xemidString.Substring(13);
            else if (xemidString.Length == 22)
                xemid.CertificationSubmissionIdentifier = xemidString.Substring(14);

            return xemid;
        }
    }
}