namespace SabreTools.Serialization.Deserializers
{
    public class ArchiveDotOrg :
        XmlFile<Models.ArchiveDotOrg.Files>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="Interfaces.IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.ArchiveDotOrg.Files? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new ArchiveDotOrg();
            return deserializer.Deserialize(data, offset);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="Interfaces.IFileDeserializer.Deserialize(string?)"/>
        public static Models.ArchiveDotOrg.Files? DeserializeFile(string? path)
        {
            var deserializer = new ArchiveDotOrg();
            return deserializer.Deserialize(path);
        }

        #endregion

        #region IStreamDeserializer

        /// <inheritdoc cref="Interfaces.IStreamDeserializer.Deserialize(Stream?)"/>
        public static Models.ArchiveDotOrg.Files? DeserializeStream(System.IO.Stream? data)
        {
            var deserializer = new ArchiveDotOrg();
            return deserializer.Deserialize(data);
        }

        #endregion
    }
}