using System.IO;
using SabreTools.Models.OpenMSX;

namespace SabreTools.Serialization.Serializers
{
    public class OpenMSX : XmlFile<SoftwareDb>
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

        #region IByteSerializer

        /// <inheritdoc cref="XmlFile.SerializeArray(T?, string?, string?, string?, string?)" />
        public override byte[]? SerializeArray(SoftwareDb? obj)
            => SerializeArray(obj, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);

        #endregion
        
        #region IFileSerializer

        /// <inheritdoc cref="XmlFile.Serialize(T?, string?, string?, string?, string?, string?)" />
        public override bool Serialize(SoftwareDb? obj, string? path)
            => Serialize(obj, path, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);

        #endregion

        #region IStreamSerializer

        /// <inheritdoc cref="XmlFile.Serialize(T?, string?, string?, string?, string?)" />
        public override Stream? Serialize(SoftwareDb? obj)
            => Serialize(obj, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);

        #endregion
    }
}