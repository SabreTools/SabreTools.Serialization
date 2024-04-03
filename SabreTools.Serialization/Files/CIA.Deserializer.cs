using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class CIA : IFileSerializer<Models.N3DS.CIA>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.N3DS.CIA? Deserialize(string? path)
        {
            var deserializer = new CIA();
            return deserializer.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.N3DS.CIA? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.CIA().DeserializeImpl(stream);
        }
    }
}