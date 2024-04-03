using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class Quantum : IFileSerializer<Models.Quantum.Archive>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.Quantum.Archive? obj, string? path)
        {
            var serializer = new Quantum();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.Quantum.Archive? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.Quantum().Serialize(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}