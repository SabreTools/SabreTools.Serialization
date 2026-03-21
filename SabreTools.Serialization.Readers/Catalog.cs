using System.IO;
using System.Text;

namespace SabreTools.Serialization.Readers
{
    public class Catalog : JsonFile<Data.Models.Xbox.Catalog>
    {
        #region IByteReader

        /// <remarks>Catalog.js file is encoded as UTF-16 LE</remarks>
        public override Data.Models.Xbox.Catalog? Deserialize(byte[]? data, int offset)
            => Deserialize(data, offset, new UnicodeEncoding());

        #endregion

        #region IFileReader

        /// <remarks>Catalog.js file is encoded as UTF-16 LE</remarks>
        public override Data.Models.Xbox.Catalog? Deserialize(string? path)
            => Deserialize(path, new UnicodeEncoding());

        #endregion

        #region IStreamReader

        /// <remarks>Catalog.js file is encoded as UTF-16 LE</remarks>
        public override Data.Models.Xbox.Catalog? Deserialize(Stream? data)
            => Deserialize(data, new UnicodeEncoding());

        #endregion
    }
}
