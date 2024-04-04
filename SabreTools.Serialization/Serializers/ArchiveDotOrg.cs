namespace SabreTools.Serialization.Serializers
{
    public class ArchiveDotOrg : XmlFile<Models.ArchiveDotOrg.Files>
    {
        #region IFileSerializer

        /// <inheritdoc cref="Interfaces.IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.ArchiveDotOrg.Files? obj, string? path)
        {
            var serializer = new ArchiveDotOrg();
            return serializer.Serialize(obj, path);
        }

        #endregion

        #region IStreamSerializer

        /// <inheritdoc cref="Interfaces.IStreamSerializer.Serialize(T?)"/>
        public static System.IO.Stream? SerializeStream(Models.ArchiveDotOrg.Files? obj)
        {
            var serializer = new ArchiveDotOrg();
            return serializer.Serialize(obj);
        }

        #endregion
    }
}