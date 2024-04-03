namespace SabreTools.Serialization.Deserializers
{
    public class ArchiveDotOrg : XmlFile<Models.ArchiveDotOrg.Files>
    {
        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.ArchiveDotOrg.Files? DeserializeFile(string? path)
        {
            var deserializer = new ArchiveDotOrg();
            return deserializer.Deserialize(path);
        }

        #endregion
    }
}