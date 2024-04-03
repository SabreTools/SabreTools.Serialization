namespace SabreTools.Serialization.Files
{
    public partial class Logiqx : XmlFile<Models.Logiqx.Datafile>
    {
        /// <inheritdoc cref="Interfaces.IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.Logiqx.Datafile? obj, string? path)
        {
            var serializer = new Logiqx();
            return serializer.Serialize(obj, path);
        }
        
        /// <inheritdoc cref="XmlFile.Serialize(T?, string?, string?, string?, string?, string?)" />
        public override bool Serialize(Models.Logiqx.Datafile? obj, string? path)
            => Serialize(obj, path, Serialization.Logiqx.DocTypeName, Serialization.Logiqx.DocTypePubId, Serialization.Logiqx.DocTypeSysId, Serialization.Logiqx.DocTypeSysId);
    }
}