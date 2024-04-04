namespace SabreTools.Serialization.Serializers
{
    public class OfflineList :
        XmlFile<Models.OfflineList.Dat>
    {
        #region IFileSerializer

        /// <inheritdoc cref="Interfaces.IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.OfflineList.Dat? obj, string? path)
        {
            var serializer = new OfflineList();
            return serializer.Serialize(obj, path);
        }

        #endregion

        #region IStreamSerializer

        /// <inheritdoc cref="Interfaces.IStreamSerializer.Serialize(T?)"/>
        public static System.IO.Stream? SerializeStream(Models.OfflineList.Dat? obj)
        {
            var serializer = new OfflineList();
            return serializer.Serialize(obj);
        }

        #endregion
    }
}