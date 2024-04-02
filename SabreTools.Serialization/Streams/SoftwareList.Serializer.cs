using System.IO;

namespace SabreTools.Serialization.Streams
{
    public partial class SoftwareList : XmlFile<Models.SoftwareList.SoftwareList>
    {
        /// <inheritdoc cref="SerializeToStream(Models.SoftwareList.SoftwareList, string?, string?, string?, string?)" />
        public Stream? SerializeToStreamWithDocType(Models.SoftwareList.SoftwareList? obj, string path)
            => Serialize(obj, Serialization.SoftawreList.DocTypeName, Serialization.SoftawreList.DocTypePubId, Serialization.SoftawreList.DocTypeSysId, Serialization.SoftawreList.DocTypeSysId);
    }
}