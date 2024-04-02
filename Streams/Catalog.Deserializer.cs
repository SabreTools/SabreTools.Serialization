using System.IO;
using System.Text;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Streams
{
    public partial class Catalog : BaseJsonFile<Models.Xbox.Catalog>, IStreamSerializer<Models.Xbox.Catalog>
    {
        public Models.Xbox.Catalog? Deserialize(Stream? data)
        {
            return Deserialize(data, new UnicodeEncoding());
        }
    }
}
