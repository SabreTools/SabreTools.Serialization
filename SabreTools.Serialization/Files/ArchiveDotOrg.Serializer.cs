namespace SabreTools.Serialization.Files
{
    public partial class ArchiveDotOrg : XmlFile<Models.ArchiveDotOrg.Files>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.ArchiveDotOrg.Files? obj, string? path)
        {
            var serializer = new ArchiveDotOrg();
            return serializer.SerializeImpl(obj, path);
        }
    }
}