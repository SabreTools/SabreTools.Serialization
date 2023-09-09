using System.IO;
using SabreTools.Models.Logiqx;

namespace SabreTools.Serialization.Streams
{
    public partial class Logiqx : XmlFile<Models.Logiqx.Datafile>
    {
        /// <inheritdoc cref="Serialize(Datafile, string?, string?, string?, string?)" />
#if NET48
        public Stream SerializeToStreamWithDocType(Datafile obj, string path)
#else
        public Stream? SerializeToStreamWithDocType(Datafile obj, string path)
#endif
            => Serialize(obj, Serialization.Logiqx.DocTypeName, Serialization.Logiqx.DocTypePubId, Serialization.Logiqx.DocTypeSysId, Serialization.Logiqx.DocTypeSysId);
    }
}