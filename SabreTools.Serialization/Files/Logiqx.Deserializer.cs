namespace SabreTools.Serialization.Files
{
    public partial class Logiqx : XmlFile<Models.Logiqx.Datafile>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.Logiqx.Datafile? Deserialize(string? path)
        {
            var obj = new Logiqx();
            return obj.DeserializeImpl(path);
        }
    }
}