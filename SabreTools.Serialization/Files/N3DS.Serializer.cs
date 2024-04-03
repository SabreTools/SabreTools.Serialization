using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class N3DS : IFileSerializer<Models.N3DS.Cart>
    {
        /// <inheritdoc cref="IFileSerializer.Serialize(T?, string?)"/>
        public static bool SerializeFile(Models.N3DS.Cart? obj, string? path)
        {
            var serializer = new N3DS();
            return serializer.Serialize(obj, path);
        }
        
        /// <inheritdoc/>
        public bool Serialize(Models.N3DS.Cart? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.N3DS.SerializeStream(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}