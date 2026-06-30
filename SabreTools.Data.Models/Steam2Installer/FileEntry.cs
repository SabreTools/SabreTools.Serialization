using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.Steam2Installer
{

    public sealed class FileEntry
    {
        /// <summary>
        /// Offset of the filename in the string bytes array
        /// </summary>
        public uint NameOffset { get; set; }

        /// <summary>
        /// Offset of the path name in the string bytes array
        /// </summary>
        public uint PathOffset { get; set; }

        /// <summary>
        /// Steam depot id of the depot the file is a part of
        /// </summary>
        public uint DepotId { get; set; }

        /// <summary>
        /// Offset of the file within its sid volume
        /// </summary>
        public ulong Offset { get; set; }

        /// <summary>
        /// Size of the file, in bytes
        /// </summary>
        public ulong Size { get; set; }

        /// <summary>
        /// Which # installer disc the file is on
        /// </summary>
        public byte DiscNumber { get; set; }

        /// <summary>
        /// Which # sid volume the file is in
        /// </summary>
        public byte VolumeNumber { get; set; }

        /// <summary>
        /// Whether the file is encrypted or not
        /// </summary>
        /// <remarks>
        /// This flag isn't entirely reliable, as sometimes the file is encrypted in the sid without this
        /// flag being set in the sid. The reverse is not currently known to be true, though, so there at
        /// least shouldn't be any false positives when using this flag.
        /// </remarks>
        public bool Encrypted { get; set; }

        /// <summary>
        /// Unknown 1-byte value, likely just padding
        /// </summary>
        public byte Unknown { get; set; }

        // The filename and path aren't stored in the fileentry structure in the .sim, but the offsets are,
        // so it just makes sense to also parse and store the strings for the file here too.

        /// <summary>
        /// The filename
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// The file path
        /// </summary>
        public string Path { get; set; } = string.Empty;
    }
}
