using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class VPK : IFileSerializer<Models.VPK.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.VPK.File? DeserializeFile(string? path)
        {
            var deserializer = new VPK();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.VPK.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.VPK.DeserializeStream(stream);
        }
    }
}