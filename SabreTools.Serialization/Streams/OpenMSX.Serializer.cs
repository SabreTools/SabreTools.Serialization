using System.IO;
using SabreTools.Models.OpenMSX;

namespace SabreTools.Serialization.Streams
{
    public partial class OpenMSX : XmlFile<SoftwareDb>
    {
        /// <inheritdoc cref="Serialize(Datafile, string?, string?, string?, string?)" />
        public Stream? SerializeToStreamWithDocType(SoftwareDb? obj, string path)
            => Serialize(obj, Serialization.OpenMSX.DocTypeName, Serialization.OpenMSX.DocTypePubId, Serialization.OpenMSX.DocTypeSysId, Serialization.OpenMSX.DocTypeSysId);
    }
}