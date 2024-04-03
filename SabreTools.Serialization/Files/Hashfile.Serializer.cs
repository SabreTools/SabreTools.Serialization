using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class Hashfile : IFileSerializer<Models.Hashfile.Hashfile>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.Hashfile.Hashfile? obj, string? path)
        {
            var serializer = new Hashfile();
            return serializer.SerializeImpl(obj, path);
        }

        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.Hashfile.Hashfile? obj, string? path, Hash hash)
        {
            var serializer = new Hashfile();
            return serializer.SerializeImpl(obj, path, hash);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.Hashfile.Hashfile? obj, string? path)
            => SerializeImpl(obj, path, Hash.CRC);

        /// <inheritdoc/>
        public bool SerializeImpl(Models.Hashfile.Hashfile? obj, string? path, Hash hash)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.Hashfile().Serialize(obj, hash);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}