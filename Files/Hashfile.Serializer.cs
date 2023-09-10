namespace SabreTools.Serialization.Files
{
    public partial class Hashfile : IFileSerializer<Models.Hashfile.Hashfile>
    {
        /// <inheritdoc/>
#if NET48
        public bool Serialize(Models.Hashfile.Hashfile obj, string path)
#else
        public bool Serialize(Models.Hashfile.Hashfile? obj, string? path)
#endif
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            using (var stream = new Streams.Hashfile().Serialize(obj))
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