namespace SabreTools.Serialization.Streams
{
    public partial class ArchiveDotOrg : XmlFile<Models.ArchiveDotOrg.Files>
    {
        /// <inheritdoc cref="Interfaces.IStreamSerializer.Deserialize(Stream?)"/>
        public static Models.ArchiveDotOrg.Files? DeserializeStream(System.IO.Stream? data)
        {
            var deserializer = new ArchiveDotOrg();
            return deserializer.Deserialize(data);
        }
    }
}