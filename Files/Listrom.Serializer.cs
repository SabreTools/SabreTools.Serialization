using SabreTools.Models.Listrom;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class Listrom : IFileSerializer<MetadataFile>
    {
        /// <inheritdoc/>
#if NET48
        public bool Serialize(MetadataFile obj, string path)
#else
        public bool Serialize(MetadataFile? obj, string? path)
#endif
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            using (var stream = new Streams.Listrom().Serialize(obj))
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