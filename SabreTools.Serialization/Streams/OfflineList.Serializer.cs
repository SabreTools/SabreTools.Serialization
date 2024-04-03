namespace SabreTools.Serialization.Streams
{
    public partial class OfflineList : XmlFile<Models.OfflineList.Dat>
    {
        /// <inheritdoc cref="Interfaces.IStreamSerializer.Serialize(T?)"/>
        public static System.IO.Stream? SerializeStream(Models.OfflineList.Dat? obj)
        {
            var serializer = new OfflineList();
            return serializer.Serialize(obj);
        }
    }
}