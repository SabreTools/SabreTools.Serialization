using SabreTools.Models.OpenMSX;

namespace SabreTools.Serialization.Files
{
    public partial class OpenMSX : XmlFile<SoftwareDb>
    {
        /// <inheritdoc cref="Serialize(SoftwareDb, string, string?, string?, string?, string?)" />
        public bool SerializeToFileWithDocType(SoftwareDb? obj, string path)
            => Serialize(obj, path, Serialization.OpenMSX.DocTypeName, Serialization.OpenMSX.DocTypePubId, Serialization.OpenMSX.DocTypeSysId, Serialization.OpenMSX.DocTypeSysId);
    }
}