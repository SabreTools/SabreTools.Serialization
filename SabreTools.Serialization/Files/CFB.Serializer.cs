using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class CFB : IFileSerializer<Models.CFB.Binary>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.CFB.Binary? obj, string? path)
        {
            var serializer = new CFB();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.CFB.Binary? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.CFB().Serialize(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}