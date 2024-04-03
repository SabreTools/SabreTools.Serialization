namespace SabreTools.Serialization.Files
{
    public partial class OpenMSX : XmlFile<Models.OpenMSX.SoftwareDb>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.OpenMSX.SoftwareDb? Deserialize(string? path)
        {
            var obj = new OpenMSX();
            return obj.DeserializeImpl(path);
        }
    }
}