using System.Text;

namespace SabreTools.Serialization.Files
{
    public partial class Catalog : JsonFile<Models.Xbox.Catalog>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.Xbox.Catalog? obj, string? path)
        {
            var serializer = new Catalog();
            return serializer.SerializeImpl(obj, path);
        }
        
        // Catalog.js file is a UTF-16 LE JSON
        public override bool SerializeImpl(Models.Xbox.Catalog? obj, string? path)
            => SerializeImpl(obj, path, new UnicodeEncoding());
    }
}
