using System.Text;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Serializers
{
    public partial class XMID :
        IStringSerializer<Models.Xbox.XMID>
    {
        #region IStringSerializer

        /// <inheritdoc cref="IStringSerializer.Serialize(T?)"/>
        public static string? SerializeString(Models.Xbox.XMID? obj)
        {
            var deserializer = new XMID();
            return deserializer.Serialize(obj);
        }

        /// <inheritdoc/>
        public string? Serialize(Models.Xbox.XMID? obj)
        {
            if (obj == null)
                return null;

            var sb = new StringBuilder();

            sb.Append(obj.PublisherIdentifier);
            sb.Append(obj.GameID);
            sb.Append(obj.VersionNumber);
            sb.Append(obj.RegionIdentifier);

            return sb.ToString();
        }

        #endregion
    }
}