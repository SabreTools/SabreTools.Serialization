using System.IO;
using SabreTools.Models.OpenMSX;

namespace SabreTools.Serialization.Streams
{
    public partial class OpenMSX : XmlFile<SoftwareDb>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(SoftwareDb? obj)
        {
            var serializer = new OpenMSX();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc cref="XmlFile.SerializeImpl(T?, string?, string?, string?, string?)" />
        public override Stream? SerializeImpl(SoftwareDb? obj)
            => SerializeImpl(obj, Serialization.OpenMSX.DocTypeName, Serialization.OpenMSX.DocTypePubId, Serialization.OpenMSX.DocTypeSysId, Serialization.OpenMSX.DocTypeSysId);
    }
}