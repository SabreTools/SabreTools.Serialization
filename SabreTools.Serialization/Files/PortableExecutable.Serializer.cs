using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PortableExecutable : IFileSerializer<Models.PortableExecutable.Executable>
    {
        /// <inheritdoc/>
        public bool Serialize(Models.PortableExecutable.Executable? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.PortableExecutable().Serialize(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}