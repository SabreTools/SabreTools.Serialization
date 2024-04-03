using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class NCF : IFileSerializer<Models.NCF.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.NCF.File? Deserialize(string? path)
        {
            var obj = new NCF();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.NCF.File? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.NCF().Deserialize(stream);
        }
    }
}