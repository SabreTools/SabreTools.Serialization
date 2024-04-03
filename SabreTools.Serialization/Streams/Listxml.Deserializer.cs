namespace SabreTools.Serialization.Streams
{
    public partial class Listxml : XmlFile<Models.Listxml.Mame>
    {
        /// <inheritdoc cref="Interfaces.IStreamSerializer.DeserializeImpl(Stream?)"/>
        public static Models.Listxml.Mame? Deserialize(System.IO.Stream? data)
        {
            var deserializer = new Listxml();
            return deserializer.DeserializeImpl(data);
        }
    }
}