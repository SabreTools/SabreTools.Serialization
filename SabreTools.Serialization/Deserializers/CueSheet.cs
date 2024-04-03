using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class CueSheet : IFileDeserializer<Models.CueSheets.CueSheet>
    {
        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
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

        #endregion
    }
}
