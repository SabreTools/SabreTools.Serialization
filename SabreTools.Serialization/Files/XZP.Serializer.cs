using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class XZP : IFileSerializer<Models.XZP.File>
    {
        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.XZP.File? obj, string? path)
        {
            var serializer = new XZP();
            return serializer.Serialize(obj, path);
        }
        
        /// <inheritdoc/>
        public bool Serialize(Models.XZP.File? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.XZP.SerializeStream(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}