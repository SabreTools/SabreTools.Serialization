using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class CFB : IFileSerializer<Models.CFB.Binary>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.CFB.Binary? DeserializeFile(string? path)
        {
            var deserializer = new CFB();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.CFB.Binary? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.CFB.DeserializeStream(stream);
        }
    }
}