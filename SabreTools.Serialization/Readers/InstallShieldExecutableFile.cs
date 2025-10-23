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
        public static FileEntry? ParseFileEntry(Stream? data)
        {
            var obj = new FileEntry();

            obj.Name = data.ReadNullTerminatedAnsiString();
            if (obj.Name == null)
                return null;

            obj.Path = data.ReadNullTerminatedAnsiString();
            if (obj.Path == null)
                return null;

            obj.Version = data.ReadNullTerminatedAnsiString();
            if (obj.Version == null)
                return null;

            var lengthString = data.ReadNullTerminatedAnsiString();
            if (lengthString == null || !ulong.TryParse(lengthString, out var lengthValue))
                return null;

            obj.Length = lengthValue;

            return obj;
        }
    }
}
