using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class GCF : IFileSerializer<Models.GCF.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.GCF.File? DeserializeFile(string? path)
        {
            var deserializer = new GCF();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.GCF.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.GCF.DeserializeStream(stream);
        }
    }
}