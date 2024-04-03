namespace SabreTools.Serialization.Files
{
    public partial class OpenMSX : XmlFile<Models.OpenMSX.SoftwareDb>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.OpenMSX.SoftwareDb? DeserializeFile(string? path)
        {
            var deserializer = new OpenMSX();
            return deserializer.Deserialize(path);
        }
    }
}