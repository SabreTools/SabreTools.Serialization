using System.IO;
using SabreTools.Models.Logiqx;

namespace SabreTools.Serialization.Streams
{
    public partial class Logiqx : XmlFile<Datafile>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Datafile? obj)
        {
            var serializer = new Logiqx();
            return serializer.Serialize(obj);
        }
        
        /// <inheritdoc cref="XmlFile.Serialize(T?, string?, string?, string?, string?)" />
        public override Stream? Serialize(Datafile? obj)
            => Serialize(obj, Serialization.Logiqx.DocTypeName, Serialization.Logiqx.DocTypePubId, Serialization.Logiqx.DocTypeSysId, Serialization.Logiqx.DocTypeSysId);
    }
}