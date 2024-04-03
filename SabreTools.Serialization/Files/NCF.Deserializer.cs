using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class NCF : IFileSerializer<Models.NCF.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.NCF.File? DeserializeFile(string? path)
        {
            var deserializer = new NCF();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.NCF.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.NCF.DeserializeStream(stream);
        }
    }
}