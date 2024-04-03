namespace SabreTools.Serialization.Streams
{
    public partial class Logiqx : XmlFile<Models.Logiqx.Datafile>
    {
        /// <inheritdoc cref="Interfaces.IStreamSerializer.DeserializeImpl(Stream?)"/>
        public static Models.Logiqx.Datafile? Deserialize(System.IO.Stream? data)
        {
            var deserializer = new Logiqx();
            return deserializer.DeserializeImpl(data);
        }
    }
}