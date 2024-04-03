using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class BDPlus : IFileSerializer<Models.BDPlus.SVM>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.BDPlus.SVM? DeserializeFile(string? path)
        {
            var deserializer = new BDPlus();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.BDPlus.SVM? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.BDPlus.DeserializeStream(stream);
        }
    }
}