namespace SabreTools.Serialization.Streams
{
    public partial class ArchiveDotOrg : XmlFile<Models.ArchiveDotOrg.Files>
    {
        /// <inheritdoc cref="Interfaces.IStreamSerializer.DeserializeImpl(Stream?)"/>
        public static Models.ArchiveDotOrg.Files? Deserialize(System.IO.Stream? data)
        {
            var deserializer = new ArchiveDotOrg();
            return deserializer.DeserializeImpl(data);
        }
    }
}