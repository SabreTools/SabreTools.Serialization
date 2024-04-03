using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class SGA : IFileSerializer<Models.SGA.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.SGA.File? DeserializeFile(string? path)
        {
            var deserializer = new SGA();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.SGA.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.SGA.DeserializeStream(stream);
        }
    }
}