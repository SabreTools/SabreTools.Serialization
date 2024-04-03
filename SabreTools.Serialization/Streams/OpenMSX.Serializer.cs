using System.IO;
using SabreTools.Models.OpenMSX;

namespace SabreTools.Serialization.Streams
{
    public partial class OpenMSX : XmlFile<SoftwareDb>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(SoftwareDb? obj)
        {
            var serializer = new OpenMSX();
            return serializer.Serialize(obj);
        }
        
        /// <inheritdoc cref="XmlFile.Serialize(T?, string?, string?, string?, string?)" />
        public override Stream? Serialize(SoftwareDb? obj)
            => Serialize(obj, Serialization.OpenMSX.DocTypeName, Serialization.OpenMSX.DocTypePubId, Serialization.OpenMSX.DocTypeSysId, Serialization.OpenMSX.DocTypeSysId);
    }
}