namespace SabreTools.Serialization.Streams
{
    public partial class SoftwareList : XmlFile<Models.SoftwareList.SoftwareList>
    {
        /// <inheritdoc cref="Interfaces.IStreamSerializer.Deserialize(Stream?)"/>
        public static Models.SoftwareList.SoftwareList? DeserializeStream(System.IO.Stream? data)
        {
            var deserializer = new SoftwareList();
            return deserializer.Deserialize(data);
        }
    }
}