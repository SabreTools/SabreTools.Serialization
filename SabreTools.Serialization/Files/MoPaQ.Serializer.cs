using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class MoPaQ : IFileSerializer<Models.MoPaQ.Archive>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.MoPaQ.Archive? obj, string? path)
        {
            var serializer = new MoPaQ();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.MoPaQ.Archive? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.MoPaQ.Serialize(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}