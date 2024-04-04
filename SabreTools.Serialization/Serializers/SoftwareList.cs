using System.IO;

namespace SabreTools.Serialization.Serializers
{
    public class SoftwareList :
        XmlFile<Models.SoftwareList.SoftwareList>
    {
        #region Constants

        /// <summary>
        /// name field for DOCTYPE
        /// </summary>
        public const string DocTypeName = "softwarelist";

        /// <summary>
        /// pubid field for DOCTYPE
        /// </summary>
        public const string? DocTypePubId = null;

        /// <summary>
        /// sysid field for DOCTYPE
        /// </summary>
        public const string DocTypeSysId = "softwarelist.dtd";

        /// <summary>
        /// subset field for DOCTYPE
        /// </summary>
        public const string? DocTypeSubset = null;

        #endregion

        #region IFileSerializer

        /// <inheritdoc cref="Interfaces.IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.SoftwareList.SoftwareList? obj, string? path)
        {
            var serializer = new SoftwareList();
            return serializer.Serialize(obj, path);
        }
        
        /// <inheritdoc cref="XmlFile.Serialize(T?, string?, string?, string?, string?, string?)" />
        public override bool Serialize(Models.SoftwareList.SoftwareList? obj, string? path)
            => Serialize(obj, path, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);

        #endregion

        #region IStreamSerializer

        /// <inheritdoc cref="Interfaces.IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Models.SoftwareList.SoftwareList? obj)
        {
            var serializer = new SoftwareList();
            return serializer.Serialize(obj);
        }

        /// <inheritdoc cref="XmlFile.Serialize(T?, string?, string?, string?, string?)" />
        public override Stream? Serialize(Models.SoftwareList.SoftwareList? obj)
            => Serialize(obj, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);

        #endregion
    }
}