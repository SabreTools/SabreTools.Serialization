using System.IO;
using System.Text;

namespace SabreTools.Serialization.Serializers
{
    public class Catalog : JsonFile<Data.Models.Xbox.Catalog>
    {
        /// <remarks>Catalog.js file is encoded as UTF-16 LE</remarks>
        public override Stream? SerializeStream(Data.Models.Xbox.Catalog? obj)
            => Serialize(obj, new UnicodeEncoding());
    }
}
