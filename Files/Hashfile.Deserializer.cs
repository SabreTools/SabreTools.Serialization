namespace SabreTools.Serialization.Files
{
    public partial class Hashfile : IFileSerializer<Models.Hashfile.Hashfile>
    {
        /// <inheritdoc/>
#if NET48
        public Models.Hashfile.Hashfile Deserialize(string path)
#else
        public Models.Hashfile.Hashfile? Deserialize(string? path)
#endif
        {
            using (var stream = PathProcessor.OpenStream(path))
            {
                return new Streams.Hashfile().Deserialize(stream);
            }
        }
    }
}