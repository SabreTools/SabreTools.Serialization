using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class MSDOS : IFileSerializer<Models.MSDOS.Executable>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.MSDOS.Executable? obj, string? path)
        {
            var serializer = new MSDOS();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.MSDOS.Executable? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.MSDOS().SerializeImpl(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}