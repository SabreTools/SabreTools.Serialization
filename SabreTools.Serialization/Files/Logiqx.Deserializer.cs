namespace SabreTools.Serialization.Files
{
    public partial class Logiqx : XmlFile<Models.Logiqx.Datafile>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.Logiqx.Datafile? DeserializeFile(string? path)
        {
            var deserializer = new Logiqx();
            return deserializer.Deserialize(path);
        }
    }
}