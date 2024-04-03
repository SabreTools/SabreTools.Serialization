namespace SabreTools.Serialization.Deserializers
{
    public class M1 : XmlFile<Models.Listxml.M1>
    {
        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.Listxml.M1? DeserializeFile(string? path)
        {
            var deserializer = new M1();
            return deserializer.Deserialize(path);
        }

        #endregion
    }
}