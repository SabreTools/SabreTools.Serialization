using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class VBSP : IFileSerializer<Models.VBSP.File>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.VBSP.File? obj, string? path)
        {
            var serializer = new VBSP();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.VBSP.File? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.VBSP().Serialize(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}