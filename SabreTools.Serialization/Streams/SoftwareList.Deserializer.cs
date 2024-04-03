namespace SabreTools.Serialization.Streams
{
    public partial class SoftwareList : XmlFile<Models.SoftwareList.SoftwareList>
    {
        /// <inheritdoc cref="Interfaces.IStreamSerializer.DeserializeImpl(Stream?)"/>
        public static Models.SoftwareList.SoftwareList? Deserialize(System.IO.Stream? data)
        {
            var deserializer = new SoftwareList();
            return deserializer.DeserializeImpl(data);
        }
    }
}