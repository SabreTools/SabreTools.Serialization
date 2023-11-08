using SabreTools.Models.SeparatedValue;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class SeparatedValue : IFileSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public bool Serialize(MetadataFile? obj, string? path) => Serialize(obj, path, ',');

        /// <inheritdoc/>
        public bool Serialize(MetadataFile? obj, string? path, char delim)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            using (var stream = new Streams.SeparatedValue().Serialize(obj, delim))
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