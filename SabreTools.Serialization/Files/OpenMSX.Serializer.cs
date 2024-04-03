namespace SabreTools.Serialization.Files
{
    public partial class OpenMSX : XmlFile<Models.OpenMSX.SoftwareDb>
    {
        /// <inheritdoc cref="Interfaces.IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.OpenMSX.SoftwareDb? obj, string? path)
        {
            var serializer = new OpenMSX();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc cref="XmlFile.SerializeImpl(T?, string?, string?, string?, string?, string?)" />
        public override bool SerializeImpl(Models.OpenMSX.SoftwareDb? obj, string? path)
            => SerializeImpl(obj, path, Serialization.OpenMSX.DocTypeName, Serialization.OpenMSX.DocTypePubId, Serialization.OpenMSX.DocTypeSysId, Serialization.OpenMSX.DocTypeSysId);
    }
}