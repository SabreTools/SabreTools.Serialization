using System.IO;
using System.Text;

namespace SabreTools.Serialization.Streams
{
    public partial class Catalog : JsonFile<Models.Xbox.Catalog>
    {
        /// <inheritdoc cref="IStreamSerializer.Deserialize(Stream?)"/>
        public static Models.Xbox.Catalog? DeserializeStream(Stream? data)
        {
            var deserializer = new Catalog();
            return deserializer.Deserialize(data);
        }
        
        // Catalog JSON is encoded as UTF-16 LE
        public override Models.Xbox.Catalog? Deserialize(Stream? data)
            => Deserialize(data, new UnicodeEncoding());
    }
}
