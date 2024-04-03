using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class SeparatedValue : IFileSerializer<Models.SeparatedValue.MetadataFile>
    {
        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.SeparatedValue.MetadataFile? obj, string? path)
        {
            var serializer = new SeparatedValue();
            return serializer.Serialize(obj, path);
        }

        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.SeparatedValue.MetadataFile? obj, string? path, char delim)
        {
            var serializer = new SeparatedValue();
            return serializer.Serialize(obj, path, delim);
        }
        
        /// <inheritdoc/>
        public bool Serialize(Models.SeparatedValue.MetadataFile? obj, string? path)
            => Serialize(obj, path, ',');

        /// <inheritdoc/>
        public bool Serialize(Models.SeparatedValue.MetadataFile? obj, string? path, char delim)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.SeparatedValue.SerializeStream(obj, delim);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}