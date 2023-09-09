using System.IO;
using SabreTools.Models.OpenMSX;

namespace SabreTools.Serialization.Streams
{
    public partial class OpenMSX : XmlFile<SoftwareDb>
    {
        /// <inheritdoc cref="Serialize(Datafile, string?, string?, string?, string?)" />
#if NET48
        public Stream SerializeToStreamWithDocType(SoftwareDb obj, string path)
#else
        public Stream? SerializeToStreamWithDocType(SoftwareDb? obj, string path)
#endif
            => Serialize(obj, Serialization.OpenMSX.DocTypeName, Serialization.OpenMSX.DocTypePubId, Serialization.OpenMSX.DocTypeSysId, Serialization.OpenMSX.DocTypeSysId);
    }
}