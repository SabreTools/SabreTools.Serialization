using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class VBSP : IFileSerializer<Models.VBSP.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.VBSP.File? DeserializeFile(string? path)
        {
            var deserializer = new VBSP();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.VBSP.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.VBSP.DeserializeStream(stream);
        }
    }
}