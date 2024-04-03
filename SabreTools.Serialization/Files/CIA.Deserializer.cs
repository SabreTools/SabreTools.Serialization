using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class CIA : IFileSerializer<Models.N3DS.CIA>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.N3DS.CIA? Deserialize(string? path)
        {
            var obj = new CIA();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.N3DS.CIA? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.CIA().Deserialize(stream);
        }
    }
}