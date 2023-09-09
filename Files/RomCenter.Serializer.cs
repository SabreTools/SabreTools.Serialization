using SabreTools.Models.RomCenter;

namespace SabreTools.Serialization.Files
{
    public partial class RomCenter : IFileSerializer<MetadataFile>
    {
        /// <inheritdoc/>
#if NET48
        public bool Serialize(MetadataFile obj, string path)
#else
        public bool Serialize(MetadataFile? obj, string? path)
#endif
        {
            using (var stream = new Streams.RomCenter().Serialize(obj))
            {
                if (stream == null)
                    return false;

                using (var fs = System.IO.File.OpenWrite(path))
                {
                    stream.CopyTo(fs);
                    return true;
                }
            }
        }
    }
}