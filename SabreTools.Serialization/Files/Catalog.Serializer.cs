using System.Text;

namespace SabreTools.Serialization.Files
{
    public partial class Catalog : JsonFile<Models.Xbox.Catalog>
    {
        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.Xbox.Catalog? obj, string? path)
        {
            var serializer = new Catalog();
            return serializer.Serialize(obj, path);
        }
        
        // Catalog.js file is a UTF-16 LE JSON
        public override bool Serialize(Models.Xbox.Catalog? obj, string? path)
            => Serialize(obj, path, new UnicodeEncoding());
    }
}
