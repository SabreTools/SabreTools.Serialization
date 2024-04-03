using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class PIC : IFileSerializer<Models.PIC.DiscInformation>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.PIC.DiscInformation? obj, string? path)
        {
            var serializer = new PIC();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.PIC.DiscInformation? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.PIC().SerializeImpl(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}