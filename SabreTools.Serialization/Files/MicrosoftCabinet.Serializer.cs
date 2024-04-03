using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class MicrosoftCabinet : IFileSerializer<Models.MicrosoftCabinet.Cabinet>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.MicrosoftCabinet.Cabinet? obj, string? path)
        {
            var serializer = new MicrosoftCabinet();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.MicrosoftCabinet.Cabinet? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.MicrosoftCabinet().Serialize(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}