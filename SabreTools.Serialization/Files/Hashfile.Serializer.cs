using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class Hashfile : IFileSerializer<Models.Hashfile.Hashfile>
    {
        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.Hashfile.Hashfile? obj, string? path)
        {
            var serializer = new Hashfile();
            return serializer.Serialize(obj, path);
        }

        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.Hashfile.Hashfile? obj, string? path, Hash hash)
        {
            var serializer = new Hashfile();
            return serializer.Serialize(obj, path, hash);
        }
        
        /// <inheritdoc/>
        public bool Serialize(Models.Hashfile.Hashfile? obj, string? path)
            => Serialize(obj, path, Hash.CRC);

        /// <inheritdoc/>
        public bool Serialize(Models.Hashfile.Hashfile? obj, string? path, Hash hash)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.Hashfile.SerializeStream(obj, hash);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}