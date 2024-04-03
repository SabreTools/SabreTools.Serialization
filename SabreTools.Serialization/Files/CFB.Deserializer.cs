using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class CFB : IFileSerializer<Models.CFB.Binary>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.CFB.Binary? Deserialize(string? path)
        {
            var deserializer = new CFB();
            return deserializer.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.CFB.Binary? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.CFB.Deserialize(stream);
        }
    }
}