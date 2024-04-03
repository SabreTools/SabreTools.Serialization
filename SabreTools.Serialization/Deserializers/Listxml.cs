namespace SabreTools.Serialization.Deserializers
{
    public class Listxml : XmlFile<Models.Listxml.Mame>
    {
        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.Listxml.Mame? DeserializeFile(string? path)
        {
            var deserializer = new Listxml();
            return deserializer.Deserialize(path);
        }

        #endregion
    }
}