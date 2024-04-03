using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class CueSheet : IFileSerializer<Models.CueSheets.CueSheet>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.CueSheets.CueSheet? DeserializeFile(string? path)
        {
            var deserializer = new CueSheet();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.CueSheets.CueSheet? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.CueSheet.DeserializeStream(stream);
        }
    }
}
