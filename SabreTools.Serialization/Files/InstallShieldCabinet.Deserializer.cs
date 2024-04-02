using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class InstallShieldCabinet : IFileSerializer<Models.InstallShieldCabinet.Cabinet>
    {
        /// <inheritdoc/>
        public Models.InstallShieldCabinet.Cabinet? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.InstallShieldCabinet().Deserialize(stream);
        }
    }
}