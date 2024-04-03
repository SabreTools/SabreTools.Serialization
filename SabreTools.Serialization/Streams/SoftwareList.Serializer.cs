using System.IO;

namespace SabreTools.Serialization.Streams
{
    public partial class SoftwareList : XmlFile<Models.SoftwareList.SoftwareList>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Models.SoftwareList.SoftwareList? obj)
        {
            var serializer = new SoftwareList();
            return serializer.SerializeImpl(obj);
        }
        
        /// <inheritdoc cref="XmlFile.SerializeImpl(T?, string?, string?, string?, string?)" />
        public override Stream? SerializeImpl(Models.SoftwareList.SoftwareList? obj)
            => SerializeImpl(obj, SoftawreList.DocTypeName, SoftawreList.DocTypePubId, SoftawreList.DocTypeSysId, SoftawreList.DocTypeSysId);
    }
}