using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Readers
{
    public partial class XMID : IStringReader<Data.Models.Xbox.XMID>
    {
        /// <inheritdoc cref="IStringReader.Deserialize(string?)"/>
        public static Data.Models.Xbox.XMID? DeserializeString(string? str)
        {
            var deserializer = new XMID();
            return deserializer.Deserialize(str);
        }

        /// <inheritdoc/>
        public Data.Models.Xbox.XMID? Deserialize(string? str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            string xmid = str!.TrimEnd('\0');
            if (string.IsNullOrEmpty(xmid))
                return null;

            return ParseXMID(xmid);
        }

        /// <summary>
        /// Parse an XGD2/3 XMID string
        /// </summary>
        /// <param name="xmidString">XMID string to attempt to parse</param>
        /// <returns>Filled XMID on success, null on error</returns>
        private static Data.Models.Xbox.XMID? ParseXMID(string? xmidString)
        {
            if (xmidString == null || xmidString.Length != 8)
                return null;

            var xmid = new Data.Models.Xbox.XMID();

#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
            xmid.PublisherIdentifier = xmidString[..2];
#else
            xmid.PublisherIdentifier = xmidString.Substring(0, 2);
#endif
            xmid.GameID = xmidString.Substring(2, 3);
            xmid.VersionNumber = xmidString.Substring(5, 2);
            xmid.RegionIdentifier = xmidString[7];

            return xmid;
        }
    }
}
