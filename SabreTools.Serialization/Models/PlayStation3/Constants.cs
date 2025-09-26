// TODO: Add more constants from the wiki
namespace SabreTools.Data.Models.PlayStation3
{
    /// <see href="https://psdevwiki.com/ps3/PS3_DISC.SFB"/> 
    public static class Constants
    {
        /// <summary>
        /// Identifying bytes for SFO file
        /// </summary>
        public const uint SFOMagic = 0x00505346;
        
        /// <summary>
        /// Identifying bytes for SFB file
        /// </summary>
        public const uint SFBMagic = 0x2E534642;
        
        #region Hybrid Flags

        /// <summary>
        /// Not dependant of other disc files, enables a network connection to PSN store
        /// </summary>
        public const char FlagDiscBenefits = 'S';

        /// <summary>
        /// dev_bdvd/PS3_CONTENT/THEMEDIR/PARAM.SFO
        /// </summary>
        public const char FlagThemes = 'T';

        /// <summary>
        /// dev_bdvd/PS3_CONTENT/VIDEODIR/PARAM.SFO
        /// </summary>
        public const char FlagVideo = 'V';

        /// <summary>
        /// friends? (unknown yet, usually combined with g/p?)
        /// </summary>
        public const char FlagFriends = 'f';

        /// <summary>
        /// dev_bdvd/PS3_GAME/USRDIR/PARAM.SFO
        /// dev_bdvd/PS3_EXTRA/PARAM.SFO
        /// </summary>
        public const char FlagGameExtras = 'g';

        /// <summary>
        /// music?
        /// </summary>
        public const char FlagMusic = 'm';

        /// <summary>
        /// photo? (unknown yet, used with v, fv,..)
        /// </summary>
        public const char FlagPhoto = 'p';

        /// <summary>
        /// dev_bdvd/PS3_UPDATE/PS3UPDAT.PUP
        /// </summary>
        public const char FlagFirmwareUpdate = 'u';

        /// <summary>
        /// dev_bdvd/PS3_VPRM/PARAM.SFO
        /// </summary>
        public const char FlagMovie = 'v';

        #endregion
    }
}