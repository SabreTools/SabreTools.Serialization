using System.IO;
using System.Text;

namespace SabreTools.Serialization.Streams
{
    public partial class Catalog : JsonFile<Models.Xbox.Catalog>
    {
        /// <inheritdoc cref="IStreamSerializer.SerializeImpl(T?)"/>
        public static Stream? Serialize(Models.Xbox.Catalog? obj)
        {
            var serializer = new Catalog();
            return serializer.SerializeImpl(obj);
        }
        
        // Catalog JSON is encoded as UTF-16 LE
        public override Stream? SerializeImpl(Models.Xbox.Catalog? obj)
            => SerializeImpl(obj, new UnicodeEncoding());
    }
}
