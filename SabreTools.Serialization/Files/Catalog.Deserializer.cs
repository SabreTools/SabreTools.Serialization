﻿using System.Text;

namespace SabreTools.Serialization.Files
{
    public partial class Catalog : JsonFile<Models.Xbox.Catalog>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.Xbox.Catalog? Deserialize(string? path)
        {
            var deserializer = new Catalog();
            return deserializer.DeserializeImpl(path);
        }

        // Catalog.js file is a UTF-16 LE JSON
        public override Models.Xbox.Catalog? DeserializeImpl(string? path)
            => DeserializeImpl(path, new UnicodeEncoding());
    }
}
