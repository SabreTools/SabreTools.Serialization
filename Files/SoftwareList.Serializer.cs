namespace SabreTools.Serialization.Files
{
    public partial class SoftwareList : XmlFile<Models.SoftwareList.SoftwareList>
    {
        /// <inheritdoc cref="SerializeToFile(Models.SoftwareList.SoftwareList, string, string?, string?, string?, string?)" />
        public bool SerializeToFileWithDocType(Models.SoftwareList.SoftwareList obj, string path)
            => Serialize(obj, path, Serialization.SoftawreList.DocTypeName, Serialization.SoftawreList.DocTypePubId, Serialization.SoftawreList.DocTypeSysId, Serialization.SoftawreList.DocTypeSysId);
    }
}