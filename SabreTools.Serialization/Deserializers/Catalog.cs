using System.IO;
using System.Text;

namespace SabreTools.Serialization.Deserializers
{
    public class Catalog :
        JsonFile<Models.Xbox.Catalog>
    {
        #region IByteDeserializer

        /// <inheritdoc cref="Interfaces.IByteDeserializer.Deserialize(byte[]?, int)"/>
        public static Models.Xbox.Catalog? DeserializeBytes(byte[]? data, int offset)
        {
            var deserializer = new Catalog();
            return deserializer.Deserialize(data, offset);
        }

        /// <remarks>Catalog.js file is encoded as UTF-16 LE</remarks>
        public override Models.Xbox.Catalog? Deserialize(byte[]? data, int offset)
            => Deserialize(data, offset, new UnicodeEncoding());

        #endregion

        #region IFileDeserializer

        /// <inheritdoc cref="Interfaces.IFileDeserializer.Deserialize(string?)"/>
        public static Models.Xbox.Catalog? DeserializeFile(string? path)
        {
            var deserializer = new Catalog();
            return deserializer.Deserialize(path);
        }

        /// <remarks>Catalog.js file is encoded as UTF-16 LE</remarks>
        public override Models.Xbox.Catalog? Deserialize(string? path)
            => Deserialize(path, new UnicodeEncoding());

        #endregion

        #region IStreamDeserializer

        /// <inheritdoc cref="Interfaces.IStreamDeserializer.Deserialize(Stream?)"/>
        public static Models.Xbox.Catalog? DeserializeStream(Stream? data)
        {
            var deserializer = new Catalog();
            return deserializer.Deserialize(data);
        }
        
        /// <remarks>Catalog.js file is encoded as UTF-16 LE</remarks>
        public override Models.Xbox.Catalog? Deserialize(Stream? data)
            => Deserialize(data, new UnicodeEncoding());

        #endregion
    }
}
