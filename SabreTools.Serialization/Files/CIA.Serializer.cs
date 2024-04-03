using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class CIA : IFileSerializer<Models.N3DS.CIA>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.N3DS.CIA? obj, string? path)
        {
            var serializer = new CIA();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.N3DS.CIA? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.CIA.Serialize(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}