using System.Text;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class Catalog : BaseJsonFile<Models.Xbox.Catalog>, IFileSerializer<Models.Xbox.Catalog>
    {
        public Models.Xbox.Catalog? Deserialize(string? path)
        {
            return Deserialize(path, new UnicodeEncoding());
        }
    }
}
