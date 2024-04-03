using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Deserializers
{
    public class EverdriveSMDB : IFileDeserializer<Models.EverdriveSMDB.MetadataFile>
    {
        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.EverdriveSMDB.MetadataFile? DeserializeFile(string? path)
        {
            var deserializer = new EverdriveSMDB();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.EverdriveSMDB.MetadataFile? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.EverdriveSMDB.DeserializeStream(stream);
        }

        #endregion
    }
}