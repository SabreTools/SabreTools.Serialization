namespace SabreTools.Serialization.Files
{
    public partial class Listxml : XmlFile<Models.Listxml.Mame>
    {
        /// <inheritdoc cref="Interfaces.IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.Listxml.Mame? obj, string? path)
        {
            var serializer = new Listxml();
            return serializer.SerializeImpl(obj, path);
        }
    }
}