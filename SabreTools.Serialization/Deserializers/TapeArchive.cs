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
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the current offset
                long initialOffset = data.Position;

                var archive = new Archive();

                // TODO: Implement everything

                return archive;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }
    }
}
