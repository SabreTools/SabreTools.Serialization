using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PFF : IFileSerializer<Models.PFF.Archive>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.PFF.Archive? obj, string? path)
        {
            var serializer = new PFF();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.PFF.Archive? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.PFF.Serialize(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}