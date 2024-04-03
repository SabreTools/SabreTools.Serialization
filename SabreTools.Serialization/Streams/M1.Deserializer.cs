namespace SabreTools.Serialization.Streams
{
    public partial class M1 : XmlFile<Models.Listxml.M1>
    {
        /// <inheritdoc cref="Interfaces.IStreamSerializer.DeserializeImpl(Stream?)"/>
        public static Models.Listxml.M1? Deserialize(System.IO.Stream? data)
        {
            var deserializer = new M1();
            return deserializer.DeserializeImpl(data);
        }
    }
}