namespace SabreTools.Serialization.Streams
{
    public partial class Listxml : XmlFile<Models.Listxml.Mame>
    {
        /// <inheritdoc cref="Interfaces.IStreamSerializer.Deserialize(Stream?)"/>
        public static Models.Listxml.Mame? DeserializeStream(System.IO.Stream? data)
        {
            var deserializer = new Listxml();
            return deserializer.Deserialize(data);
        }
    }
}