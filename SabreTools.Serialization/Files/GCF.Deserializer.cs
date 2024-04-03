using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class GCF : IFileSerializer<Models.GCF.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.GCF.File? Deserialize(string? path)
        {
            var obj = new GCF();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.GCF.File? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.GCF().Deserialize(stream);
        }
    }
}