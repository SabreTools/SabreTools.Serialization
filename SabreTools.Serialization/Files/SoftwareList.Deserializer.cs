namespace SabreTools.Serialization.Files
{
    public partial class SoftwareList : XmlFile<Models.SoftwareList.SoftwareList>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.SoftwareList.SoftwareList? Deserialize(string? path)
        {
            var obj = new SoftwareList();
            return obj.DeserializeImpl(path);
        }
    }
}