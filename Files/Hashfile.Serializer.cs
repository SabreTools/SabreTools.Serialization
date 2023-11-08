using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class Hashfile : IFileSerializer<Models.Hashfile.Hashfile>
    {
        /// <inheritdoc/>
        public bool Serialize(Models.Hashfile.Hashfile? obj, string? path) => Serialize(obj, path, Hash.CRC);

        /// <inheritdoc/>
        public bool Serialize(Models.Hashfile.Hashfile? obj, string? path, Hash hash)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            using (var stream = new Streams.Hashfile().Serialize(obj, hash))
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