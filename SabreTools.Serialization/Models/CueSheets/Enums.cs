using System;

/// <remarks>
/// Information sourced from http://web.archive.org/web/20070221154246/http://www.goldenhawk.com/download/cdrwin.pdf
/// </remarks>
namespace SabreTools.Serialization.Models.CueSheets
{
    /// <summary>
    /// The audio or data file’s filetype
    /// </summary>
    public enum CueFileType
    {
        /// <summary>
        /// Intel binary file (least significant byte first). Use for data files.
        /// </summary>
        BINARY,

        /// <summary>
        /// Motorola binary file (most significant byte first). Use for data files.
        /// </summary>
        MOTOROLA,

        /// <summary>
        /// Audio AIFF file (44.1KHz 16-bit stereo)
        /// </summary>
        AIFF,

        /// <summary>
        /// Audio WAVE file (44.1KHz 16-bit stereo)
        /// </summary>
        WAVE,

        /// <summary>
        /// Audio MP3 file (44.1KHz 16-bit stereo)
        /// </summary>
        MP3,
    }

    /// <summary>
    /// Track datatype
    /// </summary>
    public enum CueTrackDataType
    {
        /// <summary>
        /// AUDIO, Audio/Music (2352)
        /// </summary>
        AUDIO,

        /// <summary>
        /// CDG, Karaoke CD+G (2448)
        /// </summary>
        CDG,

        /// <summary>
        /// MODE1/2048, CD-ROM Mode1 Data (cooked)
        /// </summary>
        MODE1_2048,

        /// <summary>
        /// MODE1/2352 CD-ROM Mode1 Data (raw)
        /// </summary>
        MODE1_2352,

        /// <summary>
        /// MODE2/2336, CD-ROM XA Mode2 Data
        /// </summary>
        MODE2_2336,

        /// <summary>
        /// MODE2/2352, CD-ROM XA Mode2 Data
        /// </summary>
        MODE2_2352,

        /// <summary>
        /// CDI/2336, CD-I Mode2 Data
        /// </summary>
        CDI_2336,

        /// <summary>
        /// CDI/2352, CD-I Mode2 Data
        /// </summary>
        CDI_2352,
    }

    /// <summary>
    /// Special subcode flags within a track
    /// </summary>
    [Flags]
    public enum CueTrackFlag
    {
        /// <summary>
        /// DCP, Digital copy permitted
        /// </summary>
        DCP = 1 << 0,

        /// <summary>
        /// 4CH, Four channel audio
        /// </summary>
        FourCH = 1 << 1,

        /// <summary>
        /// PRE, Pre-emphasis enabled (audio tracks only)
        /// </summary>
        PRE = 1 << 2,

        /// <summary>
        /// SCMS, Serial Copy Management System (not supported by all recorders)
        /// </summary>
        SCMS = 1 << 3,

        /// <summary>
        /// DATA, set for data files. This flag is set automatically based on the track’s filetype
        /// </summary>
        DATA = 1 << 4,
    }
}
