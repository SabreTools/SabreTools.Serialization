namespace SabreTools.Serialization.Deserializers
{
    public class M1 :
        XmlFile<Models.Listxml.M1>
    {
        #region IFileDeserializer

        /// <inheritdoc cref="Interfaces.IFileDeserializer.Deserialize(string?)"/>
        public static Models.Listxml.M1? DeserializeFile(string? path)
        {
            var deserializer = new M1();
            return deserializer.Deserialize(path);
        }

        #endregion

        #region IStreamDeserializer

        /// <inheritdoc cref="Interfaces.IStreamDeserializer.Deserialize(Stream?)"/>
        public static Models.Listxml.M1? DeserializeStream(System.IO.Stream? data)
        {
            var deserializer = new M1();
            return deserializer.Deserialize(data);
        }

        #endregion
    }
}