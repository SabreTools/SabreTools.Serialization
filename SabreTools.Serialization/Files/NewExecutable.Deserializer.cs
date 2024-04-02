using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class NewExecutable : IFileSerializer<Models.NewExecutable.Executable>
    {
        /// <inheritdoc/>
        public Models.NewExecutable.Executable? Deserialize(string? path)
        {
            using (var stream = PathProcessor.OpenStream(path))
            {
                return new Streams.NewExecutable().Deserialize(stream);
            }
        }
    }
}