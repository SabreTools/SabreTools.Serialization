using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class N3DS : IFileSerializer<Models.N3DS.Cart>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.N3DS.Cart? obj, string? path)
        {
            var serializer = new N3DS();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.N3DS.Cart? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.N3DS().Serialize(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}