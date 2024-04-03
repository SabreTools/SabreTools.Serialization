namespace SabreTools.Serialization.Files
{
    public partial class OfflineList : XmlFile<Models.OfflineList.Dat>
    {
        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.OfflineList.Dat? obj, string? path)
        {
            var serializer = new OfflineList();
            return serializer.Serialize(obj, path);
        }
    }
}