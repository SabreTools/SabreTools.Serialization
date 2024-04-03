using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class NewExecutable : IFileSerializer<Models.NewExecutable.Executable>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.NewExecutable.Executable? DeserializeFile(string? path)
        {
            var deserializer = new NewExecutable();
            return deserializer.Deserialize(path);
        }

        /// <inheritdoc/>
        public Models.NewExecutable.Executable? Deserialize(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return Streams.NewExecutable.DeserializeStream(stream);
        }
    }
}