namespace SabreTools.Serialization.Files
{
    public partial class Logiqx : XmlFile<Models.Logiqx.Datafile>
    {
        /// <inheritdoc cref="Interfaces.IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.Logiqx.Datafile? obj, string? path)
        {
            var serializer = new Logiqx();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc cref="XmlFile.SerializeImpl(T?, string?, string?, string?, string?, string?)" />
        public override bool SerializeImpl(Models.Logiqx.Datafile? obj, string? path)
            => SerializeImpl(obj, path, Serialization.Logiqx.DocTypeName, Serialization.Logiqx.DocTypePubId, Serialization.Logiqx.DocTypeSysId, Serialization.Logiqx.DocTypeSysId);
    }
}