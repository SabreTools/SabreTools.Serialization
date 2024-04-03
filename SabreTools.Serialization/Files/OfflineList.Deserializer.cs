namespace SabreTools.Serialization.Files
{
    public partial class OfflineList : XmlFile<Models.OfflineList.Dat>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.OfflineList.Dat? DeserializeFile(string? path)
        {
            var deserializer = new OfflineList();
            return deserializer.Deserialize(path);
        }
    }
}