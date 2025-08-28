using System.IO;
using SabreTools.Models.GZIP;

namespace SabreTools.Serialization.Deserializers
{
    public class GZip : BaseBinaryDeserializer<Archive>
    {
        /// <inheritdoc/>
        protected override bool SkipCompression => true;

        /// <inheritdoc/>
        public override Archive? Deserialize(Stream? data)
        {
            throw new System.NotImplementedException();
        }
    }
}
