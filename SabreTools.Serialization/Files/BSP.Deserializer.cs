using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class BSP : IFileSerializer<Models.BSP.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.BSP.File? DeserializeFile(string? path)
        {
            var deserializer = new BSP();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.BSP.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.BSP.DeserializeStream(stream);
        }
    }
}