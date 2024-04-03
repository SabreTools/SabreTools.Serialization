using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class GCF : IFileSerializer<Models.GCF.File>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.GCF.File? obj, string? path)
        {
            var serializer = new GCF();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.GCF.File? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.GCF().SerializeImpl(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}