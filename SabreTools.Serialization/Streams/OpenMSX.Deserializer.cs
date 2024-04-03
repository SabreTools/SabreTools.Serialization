namespace SabreTools.Serialization.Streams
{
    public partial class OpenMSX : XmlFile<Models.OpenMSX.SoftwareDb>
    {
        /// <inheritdoc cref="Interfaces.IStreamSerializer.DeserializeImpl(Stream?)"/>
        public static Models.OpenMSX.SoftwareDb? Deserialize(System.IO.Stream? data)
        {
            var deserializer = new OpenMSX();
            return deserializer.DeserializeImpl(data);
        }
    }
}