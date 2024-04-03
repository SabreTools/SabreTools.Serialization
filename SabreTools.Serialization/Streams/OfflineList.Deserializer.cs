namespace SabreTools.Serialization.Streams
{
    public partial class OfflineList : XmlFile<Models.OfflineList.Dat>
    {
        /// <inheritdoc cref="Interfaces.IStreamSerializer.Deserialize(Stream?)"/>
        public static Models.OfflineList.Dat? DeserializeStream(System.IO.Stream? data)
        {
            var deserializer = new OfflineList();
            return deserializer.Deserialize(data);
        }
    }
}