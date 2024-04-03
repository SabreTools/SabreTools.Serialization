namespace SabreTools.Serialization.Streams
{
    public partial class OfflineList : XmlFile<Models.OfflineList.Dat>
    {
        /// <inheritdoc cref="Interfaces.IStreamSerializer.DeserializeImpl(Stream?)"/>
        public static Models.OfflineList.Dat? Deserialize(System.IO.Stream? data)
        {
            var deserializer = new OfflineList();
            return deserializer.DeserializeImpl(data);
        }
    }
}