using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class VPK : IFileSerializer<Models.VPK.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.VPK.File? Deserialize(string? path)
        {
            var deserializer = new VPK();
            return deserializer.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.VPK.File? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.VPK().Deserialize(stream);
        }
    }
}