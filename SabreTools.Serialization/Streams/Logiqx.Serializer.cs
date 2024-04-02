using System.IO;
using SabreTools.Models.Logiqx;

namespace SabreTools.Serialization.Streams
{
    public partial class Logiqx : XmlFile<Datafile>
    {
        /// <inheritdoc cref="Serialize(Datafile, string?, string?, string?, string?)" />
        public Stream? SerializeToStreamWithDocType(Datafile obj, string path)
            => Serialize(obj, Serialization.Logiqx.DocTypeName, Serialization.Logiqx.DocTypePubId, Serialization.Logiqx.DocTypeSysId, Serialization.Logiqx.DocTypeSysId);
    }
}