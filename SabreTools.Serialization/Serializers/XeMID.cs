using System.Text;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Serializers
{
    public partial class XeMID : IStringSerializer<Models.Xbox.XeMID>
    {
        /// <inheritdoc cref="IStringSerializer.Serialize(T?)"/>
        public static string? SerializeString(Models.Xbox.XeMID? obj)
        {
            var deserializer = new XeMID();
            return deserializer.Serialize(obj);
        }

        /// <inheritdoc/>
        public string? Serialize(Models.Xbox.XeMID? obj)
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
