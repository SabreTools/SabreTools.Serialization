using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class MSDOS : IFileSerializer<Models.MSDOS.Executable>
    {
        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.MSDOS.Executable? obj, string? path)
        {
            var serializer = new MSDOS();
            return serializer.Serialize(obj, path);
        }
        
        /// <inheritdoc/>
        public bool Serialize(Models.MSDOS.Executable? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.MSDOS.SerializeStream(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}