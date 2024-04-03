using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class IRD : IFileSerializer<Models.IRD.File>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.IRD.File? Deserialize(string? path)
        {
            var deserializer = new IRD();
            return deserializer.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.IRD.File? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.IRD().DeserializeImpl(stream);
        }
    }
}