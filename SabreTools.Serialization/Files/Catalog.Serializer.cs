using System.Text;

namespace SabreTools.Serialization.Files
{
    public partial class Catalog : JsonFile<Models.Xbox.Catalog>
    {
        // Catalog.js file is a UTF-16 LE JSON
        public new bool Serialize(Models.Xbox.Catalog? obj, string? path)
            => Serialize(obj, path, new UnicodeEncoding());
    }
}
