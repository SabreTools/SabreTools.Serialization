using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class BDPlus : IFileSerializer<Models.BDPlus.SVM>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.BDPlus.SVM? obj, string? path)
        {
            var serializer = new BDPlus();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.BDPlus.SVM? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.BDPlus.Serialize(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}