using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class NCF : IFileSerializer<Models.NCF.File>
    {
        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.NCF.File? obj, string? path)
        {
            var serializer = new NCF();
            return serializer.Serialize(obj, path);
        }
        
        /// <inheritdoc/>
        public bool Serialize(Models.NCF.File? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.NCF.SerializeStream(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}