using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class BSP : IFileSerializer<Models.BSP.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.BSP.File? Deserialize(string? path)
        {
            var deserializer = new BSP();
            return deserializer.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.BSP.File? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.BSP().DeserializeImpl(stream);
        }
    }
}