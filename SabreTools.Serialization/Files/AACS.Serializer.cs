using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class AACS : IFileSerializer<Models.AACS.MediaKeyBlock>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.AACS.MediaKeyBlock? obj, string? path)
        {
            var serializer = new AACS();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.AACS.MediaKeyBlock? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.AACS.Serialize(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}