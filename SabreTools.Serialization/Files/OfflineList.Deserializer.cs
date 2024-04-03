namespace SabreTools.Serialization.Files
{
    public partial class OfflineList : XmlFile<Models.OfflineList.Dat>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.OfflineList.Dat? Deserialize(string? path)
        {
            var obj = new OfflineList();
            return obj.DeserializeImpl(path);
        }
    }
}