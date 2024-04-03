namespace SabreTools.Serialization.Files
{
    public partial class SoftwareList : XmlFile<Models.SoftwareList.SoftwareList>
    {
        /// <inheritdoc cref="Interfaces.IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.SoftwareList.SoftwareList? obj, string? path)
        {
            var serializer = new SoftwareList();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc cref="XmlFile.SerializeImpl(T?, string?, string?, string?, string?, string?)" />
        public override bool SerializeImpl(Models.SoftwareList.SoftwareList? obj, string? path)
            => SerializeImpl(obj, path, SoftawreList.DocTypeName, SoftawreList.DocTypePubId, SoftawreList.DocTypeSysId, SoftawreList.DocTypeSysId);
    }
}