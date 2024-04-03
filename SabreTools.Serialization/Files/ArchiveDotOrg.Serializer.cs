namespace SabreTools.Serialization.Files
{
    public partial class ArchiveDotOrg : XmlFile<Models.ArchiveDotOrg.Files>
    {
        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.ArchiveDotOrg.Files? obj, string? path)
        {
            var serializer = new ArchiveDotOrg();
            return serializer.Serialize(obj, path);
        }
    }
}