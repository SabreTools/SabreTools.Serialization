namespace SabreTools.Serialization.Deserializers
{
    public class Logiqx : XmlFile<Models.Logiqx.Datafile>
    {
        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.Logiqx.Datafile? DeserializeFile(string? path)
        {
            var deserializer = new Logiqx();
            return deserializer.Deserialize(path);
        }

        #endregion
    }
}