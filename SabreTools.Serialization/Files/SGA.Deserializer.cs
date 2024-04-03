using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class SGA : IFileSerializer<Models.SGA.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.SGA.File? Deserialize(string? path)
        {
            var deserializer = new SGA();
            return deserializer.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.SGA.File? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.SGA().DeserializeImpl(stream);
        }
    }
}