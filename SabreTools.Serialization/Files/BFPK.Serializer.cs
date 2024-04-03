using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class BFPK : IFileSerializer<Models.BFPK.Archive>
    {
        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.BFPK.Archive? obj, string? path)
        {
            var serializer = new BFPK();
            return serializer.Serialize(obj, path);
        }
        
        /// <inheritdoc/>
        public bool Serialize(Models.BFPK.Archive? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.BFPK.SerializeStream(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}