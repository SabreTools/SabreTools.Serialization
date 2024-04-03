using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class VBSP : IFileSerializer<Models.VBSP.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.VBSP.File? Deserialize(string? path)
        {
            var deserializer = new VBSP();
            return deserializer.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.VBSP.File? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.VBSP.Deserialize(stream);
        }
    }
}