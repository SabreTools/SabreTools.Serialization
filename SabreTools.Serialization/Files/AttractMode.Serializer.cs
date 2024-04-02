using System.IO;
using SabreTools.Models.AttractMode;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class AttractMode : IFileSerializer<MetadataFile>
    {
        /// <inheritdoc/>
        public bool Serialize(MetadataFile? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.AttractMode().Serialize(obj);
            if (stream == null)
                return false;

            using var fs = File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}