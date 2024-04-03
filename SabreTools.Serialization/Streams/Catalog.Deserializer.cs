using System.IO;
using System.Text;

namespace SabreTools.Serialization.Streams
{
    public partial class Catalog : JsonFile<Models.Xbox.Catalog>
    {
        /// <inheritdoc cref="IStreamSerializer.DeserializeImpl(Stream?)"/>
        public static Models.Xbox.Catalog? Deserialize(Stream? data)
        {
            var deserializer = new Catalog();
            return deserializer.DeserializeImpl(data);
        }
        
        // Catalog JSON is encoded as UTF-16 LE
        public override Models.Xbox.Catalog? DeserializeImpl(Stream? data)
            => DeserializeImpl(data, new UnicodeEncoding());
    }
}
