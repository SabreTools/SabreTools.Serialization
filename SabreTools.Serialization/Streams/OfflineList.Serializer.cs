namespace SabreTools.Serialization.Streams
{
    public partial class OfflineList : XmlFile<Models.OfflineList.Dat>
    {
        /// <inheritdoc cref="Interfaces.IStreamSerializer.SerializeImpl(T?)"/>
        public static System.IO.Stream? Serialize(Models.OfflineList.Dat? obj)
        {
            var serializer = new OfflineList();
            return serializer.SerializeImpl(obj);
        }
    }
}