namespace SabreTools.Serialization.Files
{
    public partial class SoftwareList : XmlFile<Models.SoftwareList.SoftwareList>
    {
        /// <inheritdoc cref="Interfaces.IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.SoftwareList.SoftwareList? obj, string? path)
        {
            var serializer = new SoftwareList();
            return serializer.Serialize(obj, path);
        }
        
        /// <inheritdoc cref="XmlFile.Serialize(T?, string?, string?, string?, string?, string?)" />
        public override bool Serialize(Models.SoftwareList.SoftwareList? obj, string? path)
            => Serialize(obj, path, SoftawreList.DocTypeName, SoftawreList.DocTypePubId, SoftawreList.DocTypeSysId, SoftawreList.DocTypeSysId);
    }
}