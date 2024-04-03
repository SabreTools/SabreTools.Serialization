namespace SabreTools.Serialization.Streams
{
    public partial class Logiqx : XmlFile<Models.Logiqx.Datafile>
    {
        /// <inheritdoc cref="Interfaces.IStreamSerializer.Deserialize(Stream?)"/>
        public static Models.Logiqx.Datafile? DeserializeStream(System.IO.Stream? data)
        {
            var deserializer = new Logiqx();
            return deserializer.Deserialize(data);
        }
    }
}