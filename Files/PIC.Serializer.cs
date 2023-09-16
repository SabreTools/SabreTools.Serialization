using SabreTools.Models.PIC;
using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PIC : IFileSerializer<DiscInformation>
    {
        /// <inheritdoc/>
#if NET48
        public bool Serialize(DiscInformation obj, string path)
#else
        public bool Serialize(DiscInformation? obj, string? path)
#endif
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            using (var stream = new Streams.PIC().Serialize(obj))
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