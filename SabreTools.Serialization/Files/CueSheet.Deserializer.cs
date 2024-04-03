using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class CueSheet : IFileSerializer<Models.CueSheets.CueSheet>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.CueSheets.CueSheet? Deserialize(string? path)
        {
            var deserializer = new CueSheet();
            return deserializer.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.CueSheets.CueSheet? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.CueSheet().DeserializeImpl(stream);
        }
    }
}
