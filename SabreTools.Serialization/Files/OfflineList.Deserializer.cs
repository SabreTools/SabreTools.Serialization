namespace SabreTools.Serialization.Files
{
    public partial class OfflineList : XmlFile<Models.OfflineList.Dat>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.OfflineList.Dat? Deserialize(string? path)
        {
            var deserializer = new OfflineList();
            return deserializer.DeserializeImpl(path);
        }
    }
}