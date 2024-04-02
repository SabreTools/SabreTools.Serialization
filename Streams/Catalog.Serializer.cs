using System.IO;
using System.Text;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class Catalog : BaseJsonFile<Models.Xbox.Catalog>, IStreamSerializer<Models.Xbox.Catalog>
    {
        public Stream? Serialize(Models.Xbox.Catalog? obj)
        {
            return Serialize(obj, new UnicodeEncoding());
        }
    }
}
