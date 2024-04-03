using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class IRD : IFileSerializer<Models.IRD.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.IRD.File? DeserializeFile(string? path)
        {
            var deserializer = new IRD();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.IRD.File? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.IRD.DeserializeStream(stream);
        }
    }
}