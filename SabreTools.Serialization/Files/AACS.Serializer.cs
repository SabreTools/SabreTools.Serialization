using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class AACS : IFileSerializer<Models.AACS.MediaKeyBlock>
    {
        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.AACS.MediaKeyBlock? obj, string? path)
        {
            var serializer = new AACS();
            return serializer.Serialize(obj, path);
        }
        
        /// <inheritdoc/>
        public bool Serialize(Models.AACS.MediaKeyBlock? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.AACS.SerializeStream(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}