using System.IO;
using SabreTools.Models.Logiqx;

namespace SabreTools.Serialization.Serializers
{
    public class Logiqx :
        XmlFile<Datafile>
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

        #region IFileSerializer

        /// <inheritdoc cref="Interfaces.IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Datafile? obj, string? path)
        {
            var serializer = new Logiqx();
            return serializer.Serialize(obj, path);
        }
        
        /// <inheritdoc cref="XmlFile.Serialize(T?, string?, string?, string?, string?, string?)" />
        public override bool Serialize(Datafile? obj, string? path)
            => Serialize(obj, path, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);

        #endregion
        
        #region IStreamSerializer

        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Datafile? obj)
        {
            var serializer = new Logiqx();
            return serializer.Serialize(obj);
        }

        /// <inheritdoc cref="XmlFile.Serialize(T?, string?, string?, string?, string?)" />
        public override Stream? Serialize(Datafile? obj)
            => Serialize(obj, DocTypeName, DocTypePubId, DocTypeSysId, DocTypeSysId);

        #endregion
    }
}