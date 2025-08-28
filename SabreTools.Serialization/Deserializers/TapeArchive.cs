using System.IO;
using SabreTools.Models.TAR;

namespace SabreTools.Serialization.Deserializers
{
    public class TapeArchive : BaseBinaryDeserializer<Archive>
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
