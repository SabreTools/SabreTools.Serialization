using System.IO;
using SabreTools.Models.ClrMamePro;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class ClrMamePro : IFileSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public bool Serialize(MetadataFile? obj, string? path) => Serialize(obj, path, true);

        /// <inheritdoc cref="Serialize(MetadataFile, string)"/>
        public bool Serialize(MetadataFile? obj, string? path, bool quotes)
        {
            if (string.IsNullOrEmpty(path))
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