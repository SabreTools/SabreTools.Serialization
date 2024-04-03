namespace SabreTools.Serialization.Files
{
    public partial class M1 : XmlFile<Models.Listxml.M1>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.Listxml.M1? DeserializeFile(string? path)
        {
            var deserializer = new M1();
            return deserializer.Deserialize(path);
        }
    }
}