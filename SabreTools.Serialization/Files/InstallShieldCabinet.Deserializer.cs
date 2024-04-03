using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class InstallShieldCabinet : IFileSerializer<Models.InstallShieldCabinet.Cabinet>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.InstallShieldCabinet.Cabinet? DeserializeFile(string? path)
        {
            var deserializer = new InstallShieldCabinet();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.InstallShieldCabinet.Cabinet? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.InstallShieldCabinet.DeserializeStream(stream);
        }
    }
}