using System.Text;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Writers
{
    public partial class XeMID : IStringWriter<Data.Models.Xbox.XeMID>
    {
        /// <inheritdoc cref="IStringWriter.Serialize(T?)"/>
        public static string? SerializeString(Data.Models.Xbox.XeMID? obj)
        {
            var deserializer = new XeMID();
            return deserializer.Serialize(obj);
        }

        /// <inheritdoc/>
        public string? Serialize(Data.Models.Xbox.XeMID? obj)
        {
            if (obj == null)
                return null;

            var sb = new StringBuilder();

            sb.Append(obj.PublisherIdentifier);
            sb.Append(obj.PlatformIdentifier);
            sb.Append(obj.GameID);
            sb.Append(obj.SKU);
            sb.Append(obj.RegionIdentifier);
            sb.Append(obj.BaseVersion);
            sb.Append(obj.MediaSubtypeIdentifier);
            sb.Append(obj.DiscNumberIdentifier);
            if (!string.IsNullOrEmpty(obj.CertificationSubmissionIdentifier))
                sb.Append(obj.CertificationSubmissionIdentifier);

            return sb.ToString();
        }
    }
}
