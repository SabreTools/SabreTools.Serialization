namespace SabreTools.Serialization.Files
{
    public partial class ArchiveDotOrg : XmlFile<Models.ArchiveDotOrg.Files>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.ArchiveDotOrg.Files? Deserialize(string? path)
        {
            var obj = new ArchiveDotOrg();
            return obj.DeserializeImpl(path);
        }
    }
}