using SabreTools.Serialization.Interfaces;

namespace SabreTools.Serialization.Files
{
    public partial class NewExecutable : IFileSerializer<Models.NewExecutable.Executable>
    {
        /// <inheritdoc cref="IFileSerializer.SerializeImpl(T?, string?)"/>
        public static bool Serialize(Models.NewExecutable.Executable? obj, string? path)
        {
            var serializer = new NewExecutable();
            return serializer.SerializeImpl(obj, path);
        }
        
        /// <inheritdoc/>
        public bool SerializeImpl(Models.NewExecutable.Executable? obj, string? path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            using var stream = Streams.NewExecutable.Serialize(obj);
            if (stream == null)
                return false;

            using var fs = System.IO.File.OpenWrite(path);
            stream.CopyTo(fs);
            return true;
        }
    }
}