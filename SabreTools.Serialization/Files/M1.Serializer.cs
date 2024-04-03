namespace SabreTools.Serialization.Files
{
    public partial class M1 : XmlFile<Models.Listxml.M1>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.Listxml.M1? obj, string? path)
        {
            var serializer = new M1();
            return serializer.SerializeImpl(obj, path);
        }
    }
}