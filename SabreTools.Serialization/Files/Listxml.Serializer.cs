namespace SabreTools.Serialization.Files
{
    public partial class Listxml : XmlFile<Models.Listxml.Mame>
    {
        /// <inheritdoc cref="Interfaces.IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.Listxml.Mame? obj, string? path)
        {
            var serializer = new Listxml();
            return serializer.Serialize(obj, path);
        }
    }
}