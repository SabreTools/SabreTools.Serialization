using System.Text;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class Catalog : BaseJsonFile<Models.Xbox.Catalog>, IFileSerializer<Models.Xbox.Catalog>
    {
        public bool Serialize(Models.Xbox.Catalog? obj, string? path)
        {
            return Serialize(obj, path, new UnicodeEncoding());
        }
    }
}
