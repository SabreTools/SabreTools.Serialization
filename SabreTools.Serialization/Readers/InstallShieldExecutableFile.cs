using System.IO;
using SabreTools.Data.Models.InstallShieldExecutable;
using SabreTools.IO.Extensions;

namespace SabreTools.Serialization.Readers
{
    // TODO: This should parse an entire SFX, not just a single entry
    public class InstallShieldExecutableFile : BaseBinaryReader<FileEntry>
    {
        public override FileEntry? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data == null || !data.CanRead)
                return null;

            try
            {
                // Cache the initial offset
                long initialOffset = data.Position;

                // Try to parse the entry
                var fileEntry = ParseFileEntry(data);
                if (fileEntry == null)
                    return null;

                return fileEntry;
            }
            catch
            {
                // Ignore the actual error
                return null;
            }
        }

        /// <summary>
        /// Parse a Stream into a FileEntry
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled FileEntry on success, null on error</returns>
        public static FileEntry? ParseFileEntry(Stream data)
        {
            string? name = data.ReadNullTerminatedAnsiString();
            if (name == null)
                return null;

            string? path = data.ReadNullTerminatedAnsiString();
            if (path == null)
                return null;

            string? version = data.ReadNullTerminatedAnsiString();
            if (version == null)
                return null;

            var lengthString = data.ReadNullTerminatedAnsiString();
            if (lengthString == null || !ulong.TryParse(lengthString, out var lengthValue))
                return null;

            var obj = new FileEntry();

            obj.Name = name;
            obj.Path = path;
            obj.Version = version;
            obj.Length = lengthValue;

            return obj;
        }
    }
}
