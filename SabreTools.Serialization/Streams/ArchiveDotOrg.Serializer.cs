namespace SabreTools.Serialization.Streams
{
    public partial class ArchiveDotOrg : XmlFile<Models.ArchiveDotOrg.Files>
    {
        /// <inheritdoc cref="Interfaces.IStreamSerializer.Serialize(T?)"/>
        public static System.IO.Stream? SerializeStream(Models.ArchiveDotOrg.Files? obj)
        {
            var serializer = new ArchiveDotOrg();
            return serializer.Serialize(obj);
        }
    }
}