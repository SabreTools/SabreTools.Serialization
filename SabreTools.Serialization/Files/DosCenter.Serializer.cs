using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class DosCenter : IFileSerializer<Models.DosCenter.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.DosCenter.MetadataFile? obj, string? path)
        {
            var serializer = new DosCenter();
            return serializer.Serialize(obj, path);
        }
        
        /// <inheritdoc/>
        public bool Serialize(Models.DosCenter.MetadataFile? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.DosCenter.SerializeStream(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}