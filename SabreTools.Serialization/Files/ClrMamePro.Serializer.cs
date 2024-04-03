using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class ClrMamePro : IFileSerializer<Models.ClrMamePro.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.ClrMamePro.MetadataFile? obj, string? path)
        {
            var serializer = new ClrMamePro();
            return serializer.SerializeImpl(obj, path);
        }

        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.ClrMamePro.MetadataFile? obj, string? path, bool quotes)
        {
            var serializer = new ClrMamePro();
            return serializer.SerializeImpl(obj, path, quotes);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.ClrMamePro.MetadataFile? obj, string? path)
            => SerializeImpl(obj, path, true);

        /// <inheritdoc cref="SerializeImpl(Models.ClrMamePro.MetadataFile, string)"/>
        public bool SerializeImpl(Models.ClrMamePro.MetadataFile? obj, string? path, bool quotes)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.ClrMamePro.Serialize(obj, quotes);
            if (stream == null)
                return false;

            using var fs = File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}