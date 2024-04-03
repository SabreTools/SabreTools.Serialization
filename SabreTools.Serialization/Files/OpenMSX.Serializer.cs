namespace SabreTools.Serialization.Files
{
    public partial class OpenMSX : XmlFile<Models.OpenMSX.SoftwareDb>
    {
        /// <inheritdoc cref="Interfaces.IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.OpenMSX.SoftwareDb? obj, string? path)
        {
            var serializer = new OpenMSX();
            return serializer.Serialize(obj, path);
        }
        
        /// <inheritdoc cref="XmlFile.Serialize(T?, string?, string?, string?, string?, string?)" />
        public override bool Serialize(Models.OpenMSX.SoftwareDb? obj, string? path)
            => Serialize(obj, path, Serialization.OpenMSX.DocTypeName, Serialization.OpenMSX.DocTypePubId, Serialization.OpenMSX.DocTypeSysId, Serialization.OpenMSX.DocTypeSysId);
    }
}