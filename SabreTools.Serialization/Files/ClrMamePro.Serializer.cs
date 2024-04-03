using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class ClrMamePro : IFileSerializer<Models.ClrMamePro.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.ClrMamePro.MetadataFile? obj, string? path)
        {
            var serializer = new ClrMamePro();
            return serializer.Serialize(obj, path);
        }

        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.ClrMamePro.MetadataFile? obj, string? path, bool quotes)
        {
            var serializer = new ClrMamePro();
            return serializer.Serialize(obj, path, quotes);
        }
        
        /// <inheritdoc/>
        public bool Serialize(Models.ClrMamePro.MetadataFile? obj, string? path)
            => Serialize(obj, path, true);

        /// <inheritdoc cref="Serialize(Models.ClrMamePro.MetadataFile, string)"/>
        public bool Serialize(Models.ClrMamePro.MetadataFile? obj, string? path, bool quotes)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.ClrMamePro.SerializeStream(obj, quotes);
            if (stream == null)
                return false;

            using var fs = File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}