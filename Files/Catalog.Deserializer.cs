using System.Text;

namespace SabreTools.Serialization.Files
{
    public partial class Catalog : JsonFile<Models.Xbox.Catalog>
    {
        // Catalog.js file is a UTF-16 LE JSON
        public new Models.Xbox.Catalog? Deserialize(string? path)
            => Deserialize(path, new UnicodeEncoding());
    }
}
