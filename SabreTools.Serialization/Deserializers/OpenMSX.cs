namespace SabreTools.Serialization.Deserializers
{
    public class OpenMSX :
        XmlFile<Models.OpenMSX.SoftwareDb>
    {
        #region IFileDeserializer

        /// <inheritdoc cref="Interfaces.IFileDeserializer.Deserialize(string?)"/>
        public static Models.OpenMSX.SoftwareDb? DeserializeFile(string? path)
        {
            var deserializer = new OpenMSX();
            return deserializer.Deserialize(path);
        }

        #endregion

        #region IStreamDeserializer

        /// <inheritdoc cref="Interfaces.IStreamDeserializer.Deserialize(Stream?)"/>
        public static Models.OpenMSX.SoftwareDb? DeserializeStream(System.IO.Stream? data)
        {
            var deserializer = new OpenMSX();
            return deserializer.Deserialize(data);
        }

        #endregion
    }
}