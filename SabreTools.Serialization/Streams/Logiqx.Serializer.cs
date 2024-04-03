using System.IO;
using SabreTools.Models.Logiqx;

namespace SabreTools.Serialization.Streams
{
    public partial class Logiqx : XmlFile<Datafile>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Datafile? obj)
        {
            var serializer = new Logiqx();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc cref="XmlFile.SerializeImpl(T?, string?, string?, string?, string?)" />
        public override Stream? SerializeImpl(Datafile? obj)
            => SerializeImpl(obj, Serialization.Logiqx.DocTypeName, Serialization.Logiqx.DocTypePubId, Serialization.Logiqx.DocTypeSysId, Serialization.Logiqx.DocTypeSysId);
    }
}