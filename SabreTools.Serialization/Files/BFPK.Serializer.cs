using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class BFPK : IFileSerializer<Models.BFPK.Archive>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.BFPK.Archive? obj, string? path)
        {
            var serializer = new BFPK();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.BFPK.Archive? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.BFPK().SerializeImpl(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}