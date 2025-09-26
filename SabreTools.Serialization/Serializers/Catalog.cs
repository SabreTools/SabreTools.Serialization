using System.IO;
using System.Text;

namespace SabreTools.Serialization.Serializers
{
    public class Catalog : JsonFile<SabreTools.Models.Xbox.Catalog>
    {
        /// <remarks>Catalog.js file is encoded as UTF-16 LE</remarks>
        public override Stream? SerializeStream(SabreTools.Models.Xbox.Catalog? obj)
            => Serialize(obj, new UnicodeEncoding());
    }
}
