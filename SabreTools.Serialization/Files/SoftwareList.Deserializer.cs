namespace SabreTools.Serialization.Files
{
    public partial class SoftwareList : XmlFile<Models.SoftwareList.SoftwareList>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.SoftwareList.SoftwareList? Deserialize(string? path)
        {
            var deserializer = new SoftwareList();
            return deserializer.DeserializeImpl(path);
        }
    }
}