using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class MSDOS : IFileSerializer<Models.MSDOS.Executable>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.MSDOS.Executable? Deserialize(string? path)
        {
            var obj = new MSDOS();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.MSDOS.Executable? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.MSDOS().Deserialize(stream);
        }
    }
}