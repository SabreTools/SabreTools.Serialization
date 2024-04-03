using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class RomCenter : IFileSerializer<Models.RomCenter.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.RomCenter.MetadataFile? obj, string? path)
        {
            var serializer = new RomCenter();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.RomCenter.MetadataFile? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.RomCenter.Serialize(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}