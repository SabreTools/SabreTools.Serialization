using System.Text;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Strings
{
    public partial class XMID : IStringSerializer<Models.Xbox.XMID>
    {
        /// <inheritdoc/>
#if NET48
        public string Serialize(Models.Xbox.XMID obj)
#else
        public string? Serialize(Models.Xbox.XMID? obj)
#endif
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