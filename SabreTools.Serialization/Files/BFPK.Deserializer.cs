using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class BFPK : IFileSerializer<Models.BFPK.Archive>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.BFPK.Archive? Deserialize(string? path)
        {
            var deserializer = new BFPK();
            return deserializer.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.BFPK.Archive? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.BFPK().DeserializeImpl(stream);
        }
    }
}