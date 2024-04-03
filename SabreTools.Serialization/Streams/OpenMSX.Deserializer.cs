namespace SabreTools.Serialization.Streams
{
    public partial class OpenMSX : XmlFile<Models.OpenMSX.SoftwareDb>
    {
        /// <inheritdoc cref="Interfaces.IStreamSerializer.Deserialize(Stream?)"/>
        public static Models.OpenMSX.SoftwareDb? DeserializeStream(System.IO.Stream? data)
        {
            var deserializer = new OpenMSX();
            return deserializer.Deserialize(data);
        }
    }
}