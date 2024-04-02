using System.IO;
using System.Text;

namespace SabreTools.Serialization.Streams
{
    public partial class Catalog : JsonFile<Models.Xbox.Catalog>
    {
        // Catalog JSON is encoded as UTF-16 LE
        public new Models.Xbox.Catalog? Deserialize(Stream? data)
            => Deserialize(data, new UnicodeEncoding());
    }
}
