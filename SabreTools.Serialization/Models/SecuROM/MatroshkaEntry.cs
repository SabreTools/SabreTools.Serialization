namespace SabreTools.Data.Models.SecuROM
{
    public class MatroshkaEntry
    {
        /// <summary>
        /// File entry path, either 256 or 512 bytes
        /// </summary>
        /// <remarks>
        /// Versions without a key prefix are 256 bytes.
        /// Versions with key values either are 256 or 512 bytes.
        /// Stored as a string as the rest of the 256/512 bytes are just padding.
        /// </remarks>
        public string? Path { get; set; }

        /// <summary>
        /// Type of the entry data
        /// </summary>
        public MatroshkaEntryType EntryType { get; set; }

        /// <summary>
        /// Data size
        /// </summary>
        public uint Size { get; set; }

        /// <summary>
        /// Data offset within the package
        /// </summary>
        public uint Offset { get; set; }

        /// <summary>
        /// Unknown value only seen in later versions
        /// </summary>
        /// <remarks>Does not indicate that the offset is or isn't a 64-bit value</remarks>
        public uint? Unknown { get; set; }

        /// <summary>
        /// File modification time, stored in NTFS filetime.
        /// </summary>
        /// <see href="https://learn.microsoft.com/en-us/windows/win32/sysinfo/file-times"/>
        public ulong ModifiedTime { get; set; }

        /// <summary>
        /// File creation time, stored in NTFS filetime.
        /// </summary>
        /// <see href="https://learn.microsoft.com/en-us/windows/win32/sysinfo/file-times"/>
        public ulong CreatedTime { get; set; }

        /// <summary>
        /// File access time, stored in NTFS filetime.
        /// </summary>
        /// <see href="https://learn.microsoft.com/en-us/windows/win32/sysinfo/file-times"/>
        public ulong AccessedTime { get; set; }

        /// <summary>
        /// MD5 hash of the data
        /// </summary>
        /// <remarks>16 bytes</remarks>
        public byte[] MD5 { get; set; } = new byte[16];

        /// <summary>
        /// The file data, stored as a byte array
        /// </summary>
        public byte[] FileData { get; set; }
    }
}
