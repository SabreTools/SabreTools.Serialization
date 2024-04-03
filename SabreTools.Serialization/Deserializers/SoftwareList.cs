namespace SabreTools.Serialization.Deserializers
{
    public class SoftwareList : XmlFile<Models.SoftwareList.SoftwareList>
    {
        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.SoftwareList.SoftwareList? DeserializeFile(string? path)
        {
            var deserializer = new SoftwareList();
            return deserializer.Deserialize(path);
        }

        #endregion
    }
}