using System.IO;
using System.Text;

namespace SabreTools.Serialization.Streams
{
    public partial class Catalog : JsonFile<Models.Xbox.Catalog>
    {
        /// <inheritdoc cref="IStreamSerializer.Serialize(T?)"/>
        public static Stream? SerializeStream(Models.Xbox.Catalog? obj)
        {
            var serializer = new Catalog();
            return serializer.Serialize(obj);
        }
        
        // Catalog JSON is encoded as UTF-16 LE
        public override Stream? Serialize(Models.Xbox.Catalog? obj)
            => Serialize(obj, new UnicodeEncoding());
    }
}
