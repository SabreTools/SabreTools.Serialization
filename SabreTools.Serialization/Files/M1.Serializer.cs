namespace SabreTools.Serialization.Files
{
    public partial class M1 : XmlFile<Models.Listxml.M1>
    {
        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.Listxml.M1? obj, string? path)
        {
            var serializer = new M1();
            return serializer.Serialize(obj, path);
        }
    }
}