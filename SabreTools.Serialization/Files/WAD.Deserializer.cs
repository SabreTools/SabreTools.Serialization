using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class WAD : IFileSerializer<Models.WAD.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.WAD.File? Deserialize(string? path)
        {
            var obj = new WAD();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.WAD.File? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.WAD().Deserialize(stream);
        }
    }
}