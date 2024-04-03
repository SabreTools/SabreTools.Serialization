namespace SabreTools.Serialization.Files
{
    public partial class OpenMSX : XmlFile<Models.OpenMSX.SoftwareDb>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.OpenMSX.SoftwareDb? Deserialize(string? path)
        {
            var deserializer = new OpenMSX();
            return deserializer.DeserializeImpl(path);
        }
    }
}