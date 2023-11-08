using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class IRD : IFileSerializer<Models.IRD.IRD>
    {
        /// <inheritdoc/>
        public Models.IRD.IRD? Deserialize(string? path)
        {
            using (var stream = PathProcessor.OpenStream(path))
            {
                return new Streams.IRD().Deserialize(stream);
            }
        }
    }
}