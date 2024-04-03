namespace SabreTools.Serialization.Deserializers
{
    public class OfflineList : XmlFile<Models.OfflineList.Dat>
    {
        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.OfflineList.Dat? DeserializeFile(string? path)
        {
            var deserializer = new OfflineList();
            return deserializer.Deserialize(path);
        }

        #endregion
    }
}