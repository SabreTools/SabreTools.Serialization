using System.IO;

namespace SabreTools.Serialization.Streams
{
    public partial class SoftwareList : XmlFile<Models.SoftwareList.SoftwareList>
    {
        /// <inheritdoc cref="SerializeToStream(Models.SoftwareList.SoftwareList, string?, string?, string?, string?)" />
#if NET48
        public Stream SerializeToStreamWithDocType(Models.SoftwareList.SoftwareList obj, string path)
#else
        public Stream? SerializeToStreamWithDocType(Models.SoftwareList.SoftwareList? obj, string path)
#endif
            => Serialize(obj, Serialization.SoftawreList.DocTypeName, Serialization.SoftawreList.DocTypePubId, Serialization.SoftawreList.DocTypeSysId, Serialization.SoftawreList.DocTypeSysId);
    }
}