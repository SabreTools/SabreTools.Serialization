using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class LinearExecutable : IFileSerializer<Models.LinearExecutable.Executable>
    {
        /// <inheritdoc/>
        public bool Serialize(Models.LinearExecutable.Executable? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.LinearExecutable().Serialize(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}