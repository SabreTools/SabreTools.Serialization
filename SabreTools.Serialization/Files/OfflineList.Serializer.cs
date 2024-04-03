namespace SabreTools.Serialization.Files
{
    public partial class OfflineList : XmlFile<Models.OfflineList.Dat>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.OfflineList.Dat? obj, string? path)
        {
            var serializer = new OfflineList();
            return serializer.SerializeImpl(obj, path);
        }
    }
}