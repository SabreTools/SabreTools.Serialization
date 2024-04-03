using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class SeparatedValue : IFileSerializer<Models.SeparatedValue.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.SeparatedValue.MetadataFile? Deserialize(string? path)
        {
            var obj = new SeparatedValue();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.SeparatedValue.MetadataFile? Deserialize(string? path, char delim)
        {
            var obj = new SeparatedValue();
            return obj.DeserializeImpl(path, delim);
        }

        /// <inheritdoc/>
        public Models.SeparatedValue.MetadataFile? DeserializeImpl(string? path)
            => DeserializeImpl(path, ',');

        /// <inheritdoc/>
        public Models.SeparatedValue.MetadataFile? DeserializeImpl(string? path, char delim)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.SeparatedValue().Deserialize(stream, delim);
        }
    }
}