using System.Text;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Writers
{
    public partial class XMID : IStringSerializer<Data.Models.Xbox.XMID>
    {
        /// <inheritdoc cref="IStringSerializer.Serialize(T?)"/>
        public static string? SerializeString(Data.Models.Xbox.XMID? obj)
        {
            var deserializer = new XMID();
            return deserializer.Serialize(obj);
        }

        /// <inheritdoc/>
        public string? Serialize(Data.Models.Xbox.XMID? obj)
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
    }
}
