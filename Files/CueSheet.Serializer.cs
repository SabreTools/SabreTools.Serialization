using System.IO;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class CueSheet : IFileSerializer<Models.CueSheets.CueSheet>
    {
        /// <inheritdoc/>
        public bool Serialize(Models.CueSheets.CueSheet? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using (var stream = new Streams.CueSheet().Serialize(obj))
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