namespace SabreTools.Serialization.Files
{
    public partial class Listxml : XmlFile<Models.Listxml.Mame>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.Listxml.Mame? Deserialize(string? path)
        {
            var obj = new Listxml();
            return obj.DeserializeImpl(path);
        }
    }
}