using System.IO;

namespace SabreTools.Serialization.Files
{
    public partial class CueSheet : IFileSerializer<Models.CueSheets.CueSheet>
    {
        /// <inheritdoc/>
#if NET48
        public bool Serialize(Models.CueSheets.CueSheet obj, string path)
#else
        public bool Serialize(Models.CueSheets.CueSheet? obj, string? path)
#endif
        {
            if (string.IsNullOrWhiteSpace(path))
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