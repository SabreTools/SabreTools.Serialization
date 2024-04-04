using System.IO;
using SabreTools.Models.OpenMSX;

namespace SabreTools.Serialization.Serializers
{
    public class OpenMSX :
        XmlFile<SoftwareDb>
    {
        #region Constants

        /// <summary>
        /// name field for DOCTYPE
        /// </summary>
        public const string DocTypeName = "softwaredb";

        /// <summary>
        /// pubid field for DOCTYPE
        /// </summary>
        public const string? DocTypePubId = null;

        /// <summary>
        /// sysid field for DOCTYPE
        /// </summary>
        public const string DocTypeSysId = "softwaredb1.dtd";

        /// <summary>
        /// subset field for DOCTYPE
        /// </summary>
        public const string? DocTypeSubset = null;

        #endregion

        #region IFileSerializer

        /// <inheritdoc cref="Interfaces.IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(SoftwareDb? obj, string? path)
        {
            var serializer = new OpenMSX();
            return serializer.Serialize(obj, path);
        }
        
        /// <inheritdoc cref="XmlFile.Serialize(T?, string?, string?, string?, string?, string?)" />
        public override bool Serialize(SoftwareDb? obj, string? path)
            => Serialize(obj, path, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);

        #endregion

        #region IStreamSerializer

        /// <inheritdoc cref="Interfaces.IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(SoftwareDb? obj)
        {
            var serializer = new OpenMSX();
            return serializer.Serialize(obj);
        }

        /// <inheritdoc cref="XmlFile.Serialize(T?, string?, string?, string?, string?)" />
        public override Stream? Serialize(SoftwareDb? obj)
            => Serialize(obj, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);

        #endregion
    }
}