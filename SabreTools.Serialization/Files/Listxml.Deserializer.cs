namespace SabreTools.Serialization.Files
{
    public partial class Listxml : XmlFile<Models.Listxml.Mame>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.Listxml.Mame? Deserialize(string? path)
        {
            var deserializer = new Listxml();
            return deserializer.DeserializeImpl(path);
        }
    }
}