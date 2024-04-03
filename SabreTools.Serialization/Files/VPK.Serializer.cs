using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class VPK : IFileSerializer<Models.VPK.File>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.VPK.File? obj, string? path)
        {
            var serializer = new VPK();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.VPK.File? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = new Streams.VPK().SerializeImpl(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}