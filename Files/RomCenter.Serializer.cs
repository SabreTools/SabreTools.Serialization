using SabreTools.Models.RomCenter;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class RomCenter : IFileSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public bool Serialize(MetadataFile? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

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