using SabreTools.Models.Logiqx;

namespace SabreTools.Serialization.Files
{
    public partial class Logiqx : XmlFile<Datafile>
    {
        /// <inheritdoc cref="Serialize(Datafile, string, string?, string?, string?, string?)" />
        public bool SerializeToFileWithDocType(Datafile obj, string path)
            => Serialize(obj, path, Serialization.Logiqx.DocTypeName, Serialization.Logiqx.DocTypePubId, Serialization.Logiqx.DocTypeSysId, Serialization.Logiqx.DocTypeSysId);
    }
}