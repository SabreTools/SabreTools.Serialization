using System.IO;
using System.Text;

namespace SabreTools.Serialization.Deserializers
{
    public class Catalog : JsonFile<Data.Models.Xbox.Catalog>
    {
        #region IByteDeserializer

        /// <remarks>Catalog.js file is encoded as UTF-16 LE</remarks>
        public override Data.Models.Xbox.Catalog? Deserialize(byte[]? data, int offset)
            => Deserialize(data, offset, new UnicodeEncoding());

        #endregion

        #region IFileDeserializer

        /// <remarks>Catalog.js file is encoded as UTF-16 LE</remarks>
        public override Data.Models.Xbox.Catalog? Deserialize(string? path)
            => Deserialize(path, new UnicodeEncoding());

        #endregion

        #region IStreamDeserializer

        /// <remarks>Catalog.js file is encoded as UTF-16 LE</remarks>
        public override Data.Models.Xbox.Catalog? Deserialize(Stream? data)
            => Deserialize(data, new UnicodeEncoding());

        #endregion
    }
}
