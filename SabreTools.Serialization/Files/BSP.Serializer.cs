using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class BSP : IFileSerializer<Models.BSP.File>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.BSP.File? obj, string? path)
        {
            var serializer = new BSP();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.BSP.File? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.BSP.Serialize(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}