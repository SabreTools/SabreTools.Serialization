namespace SabreTools.Serialization.Deserializers
{
    public class OpenMSX : XmlFile<Models.OpenMSX.SoftwareDb>
    {
        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.OpenMSX.SoftwareDb? DeserializeFile(string? path)
        {
            var deserializer = new OpenMSX();
            return deserializer.Deserialize(path);
        }

        #endregion
    }
}