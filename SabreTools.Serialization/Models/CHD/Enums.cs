using System;

namespace SabreTools.Serialization.Models.CHD
{
    /// <see href="https://github.com/mamedev/mame/blob/master/src/lib/util/chdcodec.h"/> 
    public enum AVHuffCodec
    {
        DECOMPRESS_CONFIG = 1,
    }

    /// <see href="https://github.com/mamedev/mame/blob/master/src/lib/util/chdcodec.h"/> 
    public enum CodecType : uint
    {
        CHD_CODEC_NONE = 0,

        #region General Codecs

        /// <remarks>"zlib"</remarks>
        ZLIB = 0x7a6c6962,

        /// <remarks>"zstd"</remarks>
        ZSTD = 0x7a737464,

        /// <remarks>"lzma"</remarks>
        LZMA = 0x6c7a6d61,

        /// <remarks>"huff"</remarks>
        HUFFMAN = 0x68756666,

        /// <remarks>"flac"</remarks>
        FLAC = 0x666c6163,

        #endregion

        #region General Codecs with CD Frontend

        /// <remarks>"cdzl"</remarks>
        CD_ZLIB = 0x63647a6c,

        /// <remarks>"cdzs"</remarks>
        CD_ZSTD = 0x63647a73,

        /// <remarks>"cdlz"</remarks>
        CD_LZMA = 0x63646c7a,

        /// <remarks>"cdfl"</remarks>
        CD_FLAC = 0x6364666c,

        #endregion

        #region A/V Codecs

        /// <remarks>"avhu"</remarks>
        AVHUFF = 0x61766875,

        #endregion

        #region Pseudo-Codecs Returned by hunk_info

        /// <summary>
        /// Copy of another hunk
        /// </summary>
        CHD_CODEC_SELF = 1,

        /// <summary>
        /// Copy of a parent's hunk
        /// </summary>
        CHD_CODEC_PARENT = 2,

        /// <summary>
        /// Legacy "mini" 8-byte repeat
        /// </summary>
        CHD_CODEC_MINI = 3,

        #endregion
    }

    /// <see href="https://github.com/mamedev/mame/blob/master/src/lib/util/chd.h"/> 
    public enum CompressionType : uint
    {
        #region V1

        CHDCOMPRESSION_NONE = 0,
        CHDCOMPRESSION_ZLIB = 1,

        #endregion

        #region V3

        CHDCOMPRESSION_ZLIB_PLUS = 2,

        #endregion

        #region V4

        CHDCOMPRESSION_AV = 3,

        #endregion
    }

    /// <see href="https://github.com/mamedev/mame/blob/master/src/lib/util/chd.h"/> 
    public enum Error : uint
    {
        NO_INTERFACE = 1,
        NOT_OPEN,
        ALREADY_OPEN,
        INVALID_FILE,
        INVALID_DATA,
        REQUIRES_PARENT,
        FILE_NOT_WRITEABLE,
        CODEC_ERROR,
        INVALID_PARENT,
        HUNK_OUT_OF_RANGE,
        DECOMPRESSION_ERROR,
        COMPRESSION_ERROR,
        CANT_VERIFY,
        METADATA_NOT_FOUND,
        INVALID_METADATA_SIZE,
        UNSUPPORTED_VERSION,
        VERIFY_INCOMPLETE,
        INVALID_METADATA,
        INVALID_STATE,
        OPERATION_PENDING,
        UNSUPPORTED_FORMAT,
        UNKNOWN_COMPRESSION,
        WALKING_PARENT,
        COMPRESSING
    }

    /// <see href="https://github.com/mamedev/mame/blob/master/src/lib/util/chd.h"/> 
    [Flags]
    public enum Flags : uint
    {
        /// <summary>
        /// Set if this drive has a parent
        /// </summary>
        DriveHasParent = 0x00000001,

        /// <summary>
        /// Set if this drive allows writes
        /// </summary>
        DriveAllowsWrites = 0x00000002,
    }

    /// <see href="https://github.com/mamedev/mame/blob/master/src/lib/util/chd.h"/> 
    [Flags]
    public enum MetadataFlags : byte
    {
        /// <summary>
        /// Indicates data is checksummed
        /// </summary>
        CHD_MDFLAGS_CHECKSUM = 0x01,
    }

    /// <see href="https://github.com/mamedev/mame/blob/master/src/lib/util/chd.h"/> 
    public enum MetadataTag : uint
    {
        CHDMETATAG_WILDCARD = 0,

        #region Hard Disk

        /// <summary>
        /// Standard hard disk metadata
        /// </summary>
        /// <remarks>"GDDD"</remarks>
        HARD_DISK_METADATA_TAG = 0x47444444,

        /// <summary>
        /// Hard disk identify information
        /// </summary>
        /// <remarks>"IDNT"</remarks>
        HARD_DISK_IDENT_METADATA_TAG = 0x49444e54,

        /// <summary>
        /// Hard disk key information
        /// </summary>
        /// <remarks>"KEY "</remarks>
        HARD_DISK_KEY_METADATA_TAG = 0x4b455920,

        #endregion

        #region PCMCIA

        /// <summary>
        /// PCMCIA CIS information
        /// </summary>
        /// <remarks>"CIS "</remarks>
        PCMCIA_CIS_METADATA_TAG = 0x43495320,

        #endregion

        #region CD-ROM

        /// <remarks>"CHCD"</remarks>
        CDROM_OLD_METADATA_TAG = 0x43484344,

        /// <remarks>"CHTR"</remarks>
        CDROM_TRACK_METADATA_TAG = 0x43485452,

        /// <remarks>"CHT2"</remarks>
        CDROM_TRACK_METADATA2_TAG = 0x43485432,

        #endregion

        #region GD-ROM

        /// <remarks>"CHGT"</remarks>
        GDROM_OLD_METADATA_TAG = 0x43484754,

        /// <remarks>"CHGD"</remarks>
        GDROM_TRACK_METADATA_TAG = 0x43484744,

        #endregion

        #region DVD

        /// <summary>
        /// Standard DVD metadata
        /// </summary>
        /// <remarks>"DVD "</remarks>
        DVD_METADATA_TAG = 0x44564420,

        #endregion

        #region A/V

        /// <summary>
        /// Standard A/V metadata
        /// </summary>
        /// <remarks>"AVAV"</remarks>
        AV_METADATA_TAG = 0x41564156,

        /// <summary>
        /// A/V laserdisc frame metadata
        /// </summary>
        /// <remarks>"AVLD"</remarks>
        AV_LD_METADATA_TAG = 0x41564c44,

        #endregion
    }

    /// <see href="https://github.com/mamedev/mame/blob/master/src/lib/util/chd.cpp"/> 
    public enum V34EntryType : uint
    {
        /// <summary>
        /// Invalid type
        /// </summary>
        V34_MAP_ENTRY_TYPE_INVALID = 0,

        /// <summary>
        /// Standard compression
        /// </summary>
        V34_MAP_ENTRY_TYPE_COMPRESSED = 1,

        /// <summary>
        /// Uncompressed data
        /// </summary>
        V34_MAP_ENTRY_TYPE_UNCOMPRESSED = 2,

        /// <summary>
        /// Mini: use offset as raw data
        /// </summary>
        V34_MAP_ENTRY_TYPE_MINI = 3,

        /// <summary>
        /// Same as another hunk in this file
        /// </summary>
        V34_MAP_ENTRY_TYPE_SELF_HUNK = 4,

        /// <summary>
        /// Same as a hunk in the parent file
        /// </summary>
        V34_MAP_ENTRY_TYPE_PARENT_HUNK = 5,

        /// <summary>
        /// Compressed with secondary algorithm (usually FLAC CDDA)
        /// </summary>
        V34_MAP_ENTRY_TYPE_2ND_COMPRESSED = 6,
    }

    /// <see href="https://github.com/mamedev/mame/blob/master/src/lib/util/chd.cpp"/> 
    public enum V5CompressionType : uint
    {
        // These types are live when running

        /// <summary>
        /// Codec #0
        /// </summary>
        COMPRESSION_TYPE_0 = 0,

        /// <summary>
        /// Codec #1
        /// </summary>
        COMPRESSION_TYPE_1 = 1,

        /// <summary>
        /// Codec #2
        /// </summary>
        COMPRESSION_TYPE_2 = 2,

        /// <summary>
        /// Codec #3
        /// </summary>
        COMPRESSION_TYPE_3 = 3,

        /// <summary>
        /// No compression; implicit length = hunkbytes
        /// </summary>
        COMPRESSION_NONE = 4,

        /// <summary>
        /// Same as another block in this CHD
        /// </summary>
        COMPRESSION_SELF = 5,

        /// <summary>
        /// Same as a hunk's worth of units in the parent CHD
        /// </summary>
        COMPRESSION_PARENT = 6,

        // These additional pseudo-types are used for compressed encodings

        /// <summary>
        /// Start of small RLE run (4-bit length)
        /// </summary>
        COMPRESSION_RLE_SMALL,

        /// <summary>
        /// Start of large RLE run (8-bit length)
        /// </summary>
        COMPRESSION_RLE_LARGE,

        /// <summary>
        /// Same as the last COMPRESSION_SELF block
        /// </summary>
        COMPRESSION_SELF_0,

        /// <summary>
        /// Same as the last COMPRESSION_SELF block + 1
        /// </summary>
        COMPRESSION_SELF_1,

        /// <summary>
        /// Same block in the parent
        /// </summary>
        COMPRESSION_PARENT_SELF,

        /// <summary>
        /// Same as the last COMPRESSION_PARENT block
        /// </summary>
        COMPRESSION_PARENT_0,

        /// <summary>
        /// Same as the last COMPRESSION_PARENT block + 1
        /// </summary>
        COMPRESSION_PARENT_1
    }
}