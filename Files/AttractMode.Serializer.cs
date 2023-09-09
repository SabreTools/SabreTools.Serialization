using System.IO;
using SabreTools.Models.AttractMode;

namespace SabreTools.Serialization.Files
{
    public partial class AttractMode : IFileSerializer<MetadataFile>
    {
        /// <inheritdoc/>
#if NET48
        public bool Serialize(MetadataFile obj, string path)
#else
        public bool Serialize(MetadataFile? obj, string? path)
#endif
        {
            using (var stream = new Streams.AttractMode().Serialize(obj))
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