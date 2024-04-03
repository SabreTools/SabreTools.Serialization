using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class DosCenter : IFileSerializer<Models.DosCenter.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.DosCenter.MetadataFile? obj, string? path)
        {
            var serializer = new DosCenter();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.DosCenter.MetadataFile? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.DosCenter().Serialize(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}