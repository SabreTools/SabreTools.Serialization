using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class IRD : IFileSerializer<Models.IRD.IRD>
    {
        /// <inheritdoc/>
#if NET48
        public Models.IRD.IRD Deserialize(string path)
#else
        public Models.IRD.IRD? Deserialize(string? path)
#endif
        {
            using (var stream = PathProcessor.OpenStream(path))
            {
                return new Streams.IRD().Deserialize(stream);
            }
        }
    }
}