using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class BSP : IFileSerializer<Models.BSP.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.BSP.File? Deserialize(string? path)
        {
            var obj = new BSP();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.BSP.File? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.BSP().Deserialize(stream);
        }
    }
}