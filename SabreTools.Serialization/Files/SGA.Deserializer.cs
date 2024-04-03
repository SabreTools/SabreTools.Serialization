using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class SGA : IFileSerializer<Models.SGA.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.SGA.File? Deserialize(string? path)
        {
            var obj = new SGA();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.SGA.File? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.SGA().Deserialize(stream);
        }
    }
}