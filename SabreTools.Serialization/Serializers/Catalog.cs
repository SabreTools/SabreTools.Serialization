using System.IO;
using System.Text;

namespace SabreTools.Serialization.Serializers
{
    public class Catalog :
        JsonFile<Models.Xbox.Catalog>
    {
        #region IFileSerializer

        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.Xbox.Catalog? obj, string? path)
        {
            var serializer = new Catalog();
            return serializer.Serialize(obj, path);
        }
        
        /// <remarks>Catalog.js file is encoded as UTF-16 LE</remarks>
        public override bool Serialize(Models.Xbox.Catalog? obj, string? path)
            => Serialize(obj, path, new UnicodeEncoding());

        #endregion

        #region IStreamSerializer

        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Models.Xbox.Catalog? obj)
        {
            var serializer = new Catalog();
            return serializer.Serialize(obj);
        }

        /// <remarks>Catalog.js file is encoded as UTF-16 LE</remarks>
        public override Stream? Serialize(Models.Xbox.Catalog? obj)
            => Serialize(obj, new UnicodeEncoding());

        #endregion
    }
}
