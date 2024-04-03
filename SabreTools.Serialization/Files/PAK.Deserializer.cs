using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PAK : IFileSerializer<Models.PAK.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.PAK.File? Deserialize(string? path)
        {
            var obj = new PAK();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.PAK.File? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.PAK().Deserialize(stream);
        }
    }
}