using System.IO;

namespace SabreTools.Serialization.Streams
{
    public partial class SoftwareList : XmlFile<Models.SoftwareList.SoftwareList>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Models.SoftwareList.SoftwareList? obj)
        {
            var serializer = new SoftwareList();
            return serializer.Serialize(obj);
        }
        
        /// <inheritdoc cref="XmlFile.Serialize(T?, string?, string?, string?, string?)" />
        public override Stream? Serialize(Models.SoftwareList.SoftwareList? obj)
            => Serialize(obj, SoftawreList.DocTypeName, SoftawreList.DocTypePubId, SoftawreList.DocTypeSysId, SoftawreList.DocTypeSysId);
    }
}