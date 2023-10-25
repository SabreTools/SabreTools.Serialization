using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class XMID : IFileSerializer<Models.Xbox.XMID>
    {
        /// <inheritdoc/>
        /// <remarks>This treats the input path like a parseable string</remarks>
#if NET48
        public Models.Xbox.XMID Deserialize(string path)
#else
        public Models.Xbox.XMID? Deserialize(string? path)
#endif
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            string xmid = path.TrimEnd('\0');
            if (string.IsNullOrWhiteSpace(xmid))
                return null;

            return ParseXMID(xmid);
        }

        /// <summary>
        /// Parse an XGD2/3 XMID string
        /// </summary>
        /// <param name="xmidString">XMID string to attempt to parse</param>
        /// <returns>Filled XMID on success, null on error</returns>
#if NET48
        private static Models.Xbox.XMID ParseXMID(string xmidString)
#else
        private static Models.Xbox.XMID? ParseXMID(string? xmidString)
#endif
        {
            if (xmidString == null || xmidString.Length != 8)
                return null;

            var xmid = new Models.Xbox.XMID();

            xmid.PublisherIdentifier = xmidString.Substring(0, 2);
            xmid.GameID = xmidString.Substring(2, 3);
            xmid.VersionNumber = xmidString.Substring(5, 2);
            xmid.RegionIdentifier = xmidString[7];

            return xmid;
        }
    }
}