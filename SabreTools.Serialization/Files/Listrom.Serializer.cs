using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class Listrom : IFileSerializer<Models.Listrom.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.Listrom.MetadataFile? obj, string? path)
        {
            var serializer = new Listrom();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.Listrom.MetadataFile? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.Listrom().SerializeImpl(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}