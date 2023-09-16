using System.IO;
using SabreTools.Models.ClrMamePro;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class ClrMamePro : IFileSerializer<MetadataFile>
    {
        /// <inheritdoc/>
#if NET48
        public bool Serialize(MetadataFile obj, string path) => Serialize(obj, path, true);
#else
        public bool Serialize(MetadataFile? obj, string? path) => Serialize(obj, path, true);
#endif

        /// <inheritdoc cref="Serialize(MetadataFile, string)"/>
#if NET48
        public bool Serialize(MetadataFile obj, string path, bool quotes)
#else
        public bool Serialize(MetadataFile? obj, string? path, bool quotes)
#endif
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            using (var stream = new Streams.ClrMamePro().Serialize(obj, quotes))
            {
                if (stream == null)
                    return false;

                using (var fs = File.OpenWrite(path))
                {
                    stream.CopyTo(fs);
                    return true;
                }
            }
        }
    }
}