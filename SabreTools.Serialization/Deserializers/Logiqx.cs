namespace SabreTools.Serialization.Deserializers
{
    public class Logiqx :
        XmlFile<Models.Logiqx.Datafile>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="Interfaces.IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.Logiqx.Datafile? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new Logiqx();
            return deserializer.Deserialize(data, offset);
        }

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="Interfaces.IFileDeserializer.Deserialize(string?)"/>
        public static Models.Logiqx.Datafile? DeserializeFile(string? path)
        {
            var deserializer = new Logiqx();
            return deserializer.Deserialize(path);
        }

        #endregion

        #region IStreamDeserializer

        /// <inheritdoc cref="Interfaces.IStreamDeserializer.Deserialize(Stream?)"/>
        public static Models.Logiqx.Datafile? DeserializeStream(System.IO.Stream? data)
        {
            var deserializer = new Logiqx();
            return deserializer.Deserialize(data);
        }

        #endregion
    }
}