using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class XZP : IFileSerializer<Models.XZP.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.XZP.File? DeserializeFile(string? path)
        {
            var deserializer = new XZP();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.XZP.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.XZP.DeserializeStream(stream);
        }
    }
}