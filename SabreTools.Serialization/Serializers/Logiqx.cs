using System.IO;
using SabreTools.Models.Logiqx;

namespace SabreTools.Serialization.Serializers
{
    public class Logiqx : XmlFile<Datafile>
    {
        #region Constants

        /// <summary>
        /// name field for DOCTYPE
        /// </summary>
        public const string DocTypeName = "datafile";

        /// <summary>
        /// pubid field for DOCTYPE
        /// </summary>
        public const string DocTypePubId = "-//Logiqx//DTD ROM Management Datafile//EN";

        /// <summary>
        /// sysid field for DOCTYPE
        /// </summary>
        public const string DocTypeSysId = "http://www.logiqx.com/Dats/datafile.dtd";

        /// <summary>
        /// subset field for DOCTYPE
        /// </summary>
        public const string? DocTypeSubset = null;

        #endregion

        #region IByteSerializer

        /// <inheritdoc cref="XmlFile.SerializeArray(T?, string?, string?, string?, string?)" />
        public override byte[]? SerializeArray(Datafile? obj)
            => SerializeArray(obj, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);

        #endregion

        #region IFileSerializer

        /// <inheritdoc cref="XmlFile.Serialize(T?, string?, string?, string?, string?, string?)" />
        public override bool Serialize(Datafile? obj, string? path)
            => Serialize(obj, path, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);

        #endregion

        #region IStreamSerializer

        /// <inheritdoc cref="XmlFile.Serialize(T?, string?, string?, string?, string?)" />
        public override Stream? Serialize(Datafile? obj)
            => Serialize(obj, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);

        #endregion
    }
}
