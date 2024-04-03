namespace SabreTools.Serialization.Streams
{
    public partial class ArchiveDotOrg : XmlFile<Models.ArchiveDotOrg.Files>
    {
        /// <inheritdoc cref="Interfaces.IStreamSerializer.SerializeImpl(T?)"/>
        public static System.IO.Stream? Serialize(Models.ArchiveDotOrg.Files? obj)
        {
            var serializer = new ArchiveDotOrg();
            return serializer.SerializeImpl(obj);
        }
    }
}