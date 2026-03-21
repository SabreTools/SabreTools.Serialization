using System.Collections.Generic;
using System.IO;
using SabreTools.Data.Models.InstallShieldExecutable;
using SabreTools.IO.Extensions;
using static SabreTools.Data.Models.InstallShieldExecutable.Constants;

#pragma warning disable IDE0017 // Simplify object initialization
namespace SabreTools.Serialization.Readers
{
    public class InstallShieldExecutable : BaseBinaryReader<SFX>
    {
        public override SFX? Deserialize(Stream? data)
        {
            // If the data is invalid
            if (data is null || !data.CanRead)
                return null;

            try
            {
                var sfx = new SFX();

                // Cache the initial offset
                long initialOffset = data.Position;

                var sfxList = new List<FileEntry>();

                while (data.Position < data.Length)
                {
                    // Try to parse the entry
                    var fileEntry = ParseFileEntry(data, initialOffset);
                    if (fileEntry is null)
                        break;

                    // Get the length, and make sure it won't EOF
                    long length = (long)fileEntry.Length;
                    if (length > data.Length - data.Position)
                        break;

                    data.SeekIfPossible(length, SeekOrigin.Current);
                    sfxList.Add(fileEntry);
                }

                if (sfxList.Count == 0)
                    return null;

                sfx.Entries = [.. sfxList];
                return sfx;
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
        public static FileEntry? ParseFileEntry(Stream data, long initialOffset)
        {
            string? name = data.ReadNullTerminatedAnsiString();
            if (name is null)
                return null;

            // Both of these strings indicate that this is a different kind of encrypted and/or compressed format of
            // ISEXE that is not yet supported, but will be in the future.
            // They return early because no extraction can be performed, like how MsCab currently returns if a folder
            // is LZX or Quantum.
            if (name == ISSignatureString)
                return null;

            if (name == ISSetupSignatureString)
                return null;

            string? path = data.ReadNullTerminatedAnsiString();
            if (path is null)
                return null;

            string? version = data.ReadNullTerminatedAnsiString();
            if (version is null)
                return null;

            var lengthString = data.ReadNullTerminatedAnsiString();
            if (lengthString is null || !ulong.TryParse(lengthString, out var lengthValue))
                return null;

            var obj = new FileEntry();
            obj.Name = name;
            obj.Path = path;
            obj.Version = version;
            obj.Length = lengthValue;
            obj.Offset = data.Position - initialOffset;

            return obj;
        }
    }
}
