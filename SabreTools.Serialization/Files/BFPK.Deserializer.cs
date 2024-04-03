using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class BFPK : IFileSerializer<Models.BFPK.Archive>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.BFPK.Archive? DeserializeFile(string? path)
        {
            var deserializer = new BFPK();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.BFPK.Archive? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.BFPK.DeserializeStream(stream);
        }
    }
}