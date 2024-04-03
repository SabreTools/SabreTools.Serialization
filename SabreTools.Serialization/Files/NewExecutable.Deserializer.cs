using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class NewExecutable : IFileSerializer<Models.NewExecutable.Executable>
    {
        /// <inheritdoc cref="IFileSerializer.Deserialize(string?)"/>
        public static Models.NewExecutable.Executable? Deserialize(string? path)
        {
            var obj = new NewExecutable();
            return obj.DeserializeImpl(path);
        }

        /// <inheritdoc/>
        public Models.NewExecutable.Executable? DeserializeImpl(string? path)
        {
            using var stream = PathProcessor.OpenStream(path);
            return new Streams.NewExecutable().Deserialize(stream);
        }
    }
}