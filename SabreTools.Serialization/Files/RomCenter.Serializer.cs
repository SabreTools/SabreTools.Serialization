using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class RomCenter : IFileSerializer<Models.RomCenter.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.RomCenter.MetadataFile? obj, string? path)
        {
            var serializer = new RomCenter();
            return serializer.Serialize(obj, path);
        }
        
        /// <inheritdoc/>
        public bool Serialize(Models.RomCenter.MetadataFile? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.RomCenter.SerializeStream(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}