namespace SabreTools.Serialization.Models.GZIP
{
    /// <see href="https://www.ietf.org/rfc/rfc1952.txt"/> 
    public sealed class Header
    {
        /// <summary>
        /// IDentification 1 (0x1F)
        /// </summary>
        public byte ID1 { get; set; }

        /// <summary>
        /// IDentification 2 (0x8B)
        /// </summary>
        public byte ID2 { get; set; }

        /// <summary>
        /// Compression Method
        /// </summary>
        public CompressionMethod CompressionMethod { get; set; }

        /// <summary>
        /// FLaGs
        /// </summary>
        public Flags Flags { get; set; }

        /// <summary>
        /// Modification TIME
        /// 
        /// This gives the most recent modification time of the original
        /// file being compressed.  The time is in Unix format, i.e.,
        /// seconds since 00:00:00 GMT, Jan.  1, 1970.  (Note that this
        /// may cause problems for MS-DOS and other systems that use
        /// local rather than Universal time.)  If the compressed data
        /// did not come from a file, MTIME is set to the time at which
        /// compression started.  MTIME = 0 means no time stamp is
        /// available.
        /// </summary>
        public uint LastModifiedTime { get; set; }

        /// <summary>
        /// eXtra FLags
        /// </summary>
        public ExtraFlags ExtraFlags { get; set; }

        /// <summary>
        /// Operating System
        /// 
        /// This identifies the type of file system on which compression
        /// took place.  This may be useful in determining end-of-line
        /// convention for text files.
        /// </summary>
        public OperatingSystem OperatingSystem { get; set; }

        /// <summary>
        /// eXtra LENgth
        /// 
        /// If FLG.FEXTRA is set, this gives the length of the optional
        /// extra field.
        /// </summary>
        public ushort ExtraLength { get; set; }

        /// <summary>
        /// Extra field
        /// 
        /// If the FLG.FEXTRA bit is set, an "extra field" is present in
        /// the header, with total length XLEN bytes.  It consists of a
        /// series of subfields, each of the form <see cref="ExtraFieldData"/>.
        /// </summary>
        public ExtraFieldData[]? ExtraField { get; set; }

        /// <summary>
        /// Original filename before compression, null-terminated
        /// </summary>
        public string? OriginalFileName { get; set; }

        /// <summary>
        /// File comment, null terminated
        /// </summary>
        public string? FileComment { get; set; }

        /// <summary>
        ///  The CRC16 consists of the two least significant bytes of the
        ///  CRC32 for all bytes of the gzip header up to and not including
        ///  the CRC16.
        /// </summary>
        public ushort? CRC16 { get; set; }
    }
}