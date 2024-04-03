using System.Text;

namespace SabreTools.Serialization.Deserializers
{
    public class Catalog : JsonFile<Models.Xbox.Catalog>
    {
        #region IFileDeserializer

        /// <inheritdoc cref="IFileDeserializer.Deserialize(string?)"/>
        public static Models.Xbox.Catalog? DeserializeFile(string? path)
        {
            var deserializer = new Catalog();
            return deserializer.Deserialize(path);
        }

        // Catalog.js file is a UTF-16 LE JSON
        public override Models.Xbox.Catalog? Deserialize(string? path)
            => Deserialize(path, new UnicodeEncoding());

        #endregion
    }
}
