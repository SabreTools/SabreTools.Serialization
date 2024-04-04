namespace SabreTools.Serialization.Deserializers
{
    public class SoftwareList :
        XmlFile<Models.SoftwareList.SoftwareList>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="Interfaces.IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.SoftwareList.SoftwareList? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new SoftwareList();
            return deserializer.Deserialize(data, offset);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="Interfaces.IFileDeserializer.Deserialize(string?)"/>
        public static Models.SoftwareList.SoftwareList? DeserializeFile(string? path)
        {
            var deserializer = new SoftwareList();
            return deserializer.Deserialize(path);
        }

        #endregion

        #region IStreamDeserializer

        /// <inheritdoc cref="Interfaces.IStreamDeserializer.Deserialize(Stream?)"/>
        public static Models.SoftwareList.SoftwareList? DeserializeStream(System.IO.Stream? data)
        {
            var deserializer = new SoftwareList();
            return deserializer.Deserialize(data);
        }

        #endregion
    }
}