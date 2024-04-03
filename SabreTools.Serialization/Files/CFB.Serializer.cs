using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class CFB : IFileSerializer<Models.CFB.Binary>
    {
        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.CFB.Binary? obj, string? path)
        {
            var serializer = new CFB();
            return serializer.Serialize(obj, path);
        }
        
        /// <inheritdoc/>
        public bool Serialize(Models.CFB.Binary? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.CFB.SerializeStream(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}