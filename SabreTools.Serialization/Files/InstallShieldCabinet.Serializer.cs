using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class InstallShieldCabinet : IFileSerializer<Models.InstallShieldCabinet.Cabinet>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.InstallShieldCabinet.Cabinet? obj, string? path)
        {
            var serializer = new InstallShieldCabinet();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.InstallShieldCabinet.Cabinet? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.InstallShieldCabinet.Serialize(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}