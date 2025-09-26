namespace SabreTools.Data.Models.IRD
{
    /// <see href="https://psdevwiki.com/ps3/Bluray_disc#IRD_file"/> 
    /// <see href="https://github.com/SabreTools/MPF/files/13062347/IRD.File.Format.pdf"/> 
    public class File
    {
        /// <summary>
        /// "3IRD"
        /// </summary>
        public byte[]? Magic { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        /// <remarks>Versions 6 - 9 are accepted</remarks>
        public byte Version { get; set; }

        /// <summary>
        /// The same value stored in PARAM.SFO / TITLE_ID
        /// </summary>
        /// <remarks>9 bytes, ASCII, stored without dashes</remarks>
        public string? TitleID { get; set; }

        /// <summary>
        /// Number of bytes that follow containing the title
        /// </summary>
        public byte TitleLength { get; set; }

        /// <summary>
        /// The same value stored in PARAM.SFO / TITLE
        /// </summary>
        /// <remarks><see cref="TitleLength"/> bytes, ASCII</remarks>
        public string? Title { get; set; }

        /// <summary>
        /// The same value stored in PARAM.SFO / PS3_SYSTEM_VER
        /// </summary>
        /// <remarks>4 bytes, ASCII, missing uses "0000"</remarks>
        public string? SystemVersion { get; set; }

        /// <summary>
        /// The same value stored in PARAM.SFO / VERSION
        /// </summary>
        /// <remarks>5 bytes, ASCII</remarks>
        public string? GameVersion { get; set; }

        /// <summary>
        /// The same value stored in PARAM.SFO / APP_VER
        /// </summary>
        /// <remarks>5 bytes, ASCII</remarks>
        public string? AppVersion { get; set; }

        /// <summary>
        /// Length of the gzip-compressed header data
        /// </summary>
        public uint HeaderLength { get; set; }

        /// <summary>
        /// Gzip-compressed header data
        /// </summary>
        public byte[]? Header { get; set; }

        /// <summary>
        /// Length of the gzip-compressed footer data
        /// </summary>
        public uint FooterLength { get; set; }

        /// <summary>
        /// Gzip-compressed footer data
        /// </summary>
        public byte[]? Footer { get; set; }

        /// <summary>
        /// Number of complete regions in the image
        /// </summary>
        public byte RegionCount { get; set; }

        /// <summary>
        /// MD5 hashes for all complete regions in the image
        /// </summary>
        /// <remarks><see cref="RegionCount"/> regions, 16-bytes per hash</remarks>
        public byte[][]? RegionHashes { get; set; }

        /// <summary>
        /// Number of decrypted files in the image
        /// </summary>
        public uint FileCount { get; set; }

        /// <summary>
        /// Starting sector for each decrypted file
        /// </summary>
        /// <remarks><see cref="FileCount"/> files, alternating with each <see cref="FileHashes"/> entry</remarks>
        public ulong[]? FileKeys { get; set; }

        /// <summary>
        /// MD5 hashes for all decrypted files in the image
        /// </summary>
        /// <remarks><see cref="FileCount"/> files, 16-bytes per hash, alternating with each <see cref="FileHashes"/> entry</remarks>
        public byte[][]? FileHashes { get; set; }

        /// <summary>
        /// Extra Config, usually 0x0000
        /// </summary>
        public ushort ExtraConfig { get; set; }

        /// <summary>
        /// Attachments, usually 0x0000
        /// </summary>
        public ushort Attachments { get; set; }

        /// <summary>
        /// D1 key
        /// </summary>
        /// <remarks>16 bytes</remarks>
        public byte[]? Data1Key { get; set; }

        /// <summary>
        /// D2 key
        /// </summary>
        /// <remarks>16 bytes</remarks>
        public byte[]? Data2Key { get; set; }

        /// <summary>
        /// Uncompressed PIC data
        /// </summary>
        /// <remarks>115 bytes, before D1/D2 keys on version 9</remarks>
        public byte[]? PIC { get; set; }

        /// <summary>
        /// Unique Identifier
        /// </summary>
        /// <remarks>Not present on version 6 and prior, after AppVersion on version 7</remarks>
        public uint UID { get; set; }

        /// <summary>
        /// IRD content CRC
        /// </summary>
        public uint CRC { get; set; }
    }
}
