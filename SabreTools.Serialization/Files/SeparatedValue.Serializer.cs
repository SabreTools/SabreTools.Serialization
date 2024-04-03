using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class SeparatedValue : IFileSerializer<Models.SeparatedValue.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.SeparatedValue.MetadataFile? obj, string? path)
        {
            var serializer = new SeparatedValue();
            return serializer.SerializeImpl(obj, path);
        }

        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.SeparatedValue.MetadataFile? obj, string? path, char delim)
        {
            var serializer = new SeparatedValue();
            return serializer.SerializeImpl(obj, path, delim);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.SeparatedValue.MetadataFile? obj, string? path)
            => SerializeImpl(obj, path, ',');

        /// <inheritdoc/>
        public bool SerializeImpl(Models.SeparatedValue.MetadataFile? obj, string? path, char delim)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.SeparatedValue().Serialize(obj, delim);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}