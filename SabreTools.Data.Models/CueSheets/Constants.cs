namespace SabreTools.Data.Models.CueSheets
{
    /// <summary>
    /// Cue sheet constant values
    /// </summary>
    /// <see href="http://web.archive.org/web/20070221154246/http://www.goldenhawk.com/download/cdrwin.pdf"/>
    public static class Constants
    {
        /// <summary>
        /// Commands of a cue sheet
        /// </summary>
        public static readonly string[] Commands =
        [
            "CATALOG", "CDTEXTFILE", "FILE", "FLAGS", "INDEX", "ISRC", "PERFORMER", "POSTGAP", "PREGAP", "REM",
            "SONGWRITER", "TITLE", "TRACK",
        ];
    }
}
