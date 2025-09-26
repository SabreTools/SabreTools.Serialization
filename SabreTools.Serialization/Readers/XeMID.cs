using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Readers
{
    public partial class XeMID : IStringDeserializer<Data.Models.Xbox.XeMID>
    {
        /// <inheritdoc cref="IStringDeserializer.Deserialize(string?)"/>
        public static Data.Models.Xbox.XeMID? DeserializeString(string? str)
        {
            var deserializer = new XeMID();
            return deserializer.Deserialize(str);
        }

        /// <inheritdoc/>
        public Data.Models.Xbox.XeMID? Deserialize(string? str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            string xemid = str!.TrimEnd('\0');
            if (string.IsNullOrEmpty(xemid))
                return null;

            return ParseXeMID(xemid);
        }

        /// <summary>
        /// Parse an XGD2/3 XeMID string
        /// </summary>
        /// <param name="xemidString">XeMID string to attempt to parse</param>
        /// <returns>Filled XeMID on success, null on error</returns>
        private static Data.Models.Xbox.XeMID? ParseXeMID(string? xemidString)
        {
            if (xemidString == null)
                return null;
            if (!(xemidString.Length == 13 || xemidString.Length == 14 || xemidString.Length == 21 || xemidString.Length == 22))
                return null;

            var xemid = new Data.Models.Xbox.XeMID();

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
