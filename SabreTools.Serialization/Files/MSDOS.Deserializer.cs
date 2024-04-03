using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class MSDOS : IFileSerializer<Models.MSDOS.Executable>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.MSDOS.Executable? DeserializeFile(string? path)
        {
            var deserializer = new MSDOS();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.MSDOS.Executable? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.MSDOS.DeserializeStream(stream);
        }
    }
}