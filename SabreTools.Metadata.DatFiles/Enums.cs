namespace SabreTools.Metadata.DatFiles
{
    /// <summary>
    /// DAT output formats
    /// </summary>
    public enum DatFormat : ulong
    {
        #region XML Formats

        /// <summary>
        /// Logiqx XML (using machine)
        /// </summary>
        Logiqx,

        /// <summary>
        /// Logiqx XML (using game)
        /// </summary>
        LogiqxDeprecated,

        /// <summary>
        /// MAME Softare List XML
        /// </summary>
        SoftwareList,

        /// <summary>
        /// MAME Listxml output
        /// </summary>
        Listxml,

        /// <summary>
        /// OfflineList XML
        /// </summary>
        OfflineList,

        /// <summary>
        /// SabreDAT XML
        /// </summary>
        SabreXML,

        /// <summary>
        /// openMSX Software List XML
        /// </summary>
        OpenMSX,

        /// <summary>
        /// Archive.org file list XML
        /// </summary>
        ArchiveDotOrg,

        #endregion

        #region Propietary Formats

        /// <summary>
        /// ClrMamePro custom
        /// </summary>
        ClrMamePro,

        /// <summary>
        /// RomCenter INI-based
        /// </summary>
        RomCenter,

        /// <summary>
        /// DOSCenter custom
        /// </summary>
        DOSCenter,

        /// <summary>
        /// AttractMode custom
        /// </summary>
        AttractMode,

        #endregion

        #region Standardized Text Formats

        /// <summary>
        /// ClrMamePro missfile
        /// </summary>
        MissFile,

        /// <summary>
        /// Comma-Separated Values (standardized)
        /// </summary>
        CSV,

        /// <summary>
        /// Semicolon-Separated Values (standardized)
        /// </summary>
        SSV,

        /// <summary>
        /// Tab-Separated Values (standardized)
        /// </summary>
        TSV,

        /// <summary>
        /// MAME Listrom output
        /// </summary>
        Listrom,

        /// <summary>
        /// Everdrive Packs SMDB
        /// </summary>
        EverdriveSMDB,

        /// <summary>
        /// SabreJSON
        /// </summary>
        SabreJSON,

        #endregion

        #region SFV-similar Formats

        /// <summary>
        /// CRC32 hash list
        /// </summary>
        RedumpSFV,

        /// <summary>
        /// MD2 hash list
        /// </summary>
        RedumpMD2,

        /// <summary>
        /// MD4 hash list
        /// </summary>
        RedumpMD4,

        /// <summary>
        /// MD5 hash list
        /// </summary>
        RedumpMD5,

        /// <summary>
        /// RIPEMD128 hash list
        /// </summary>
        RedumpRIPEMD128,

        /// <summary>
        /// RIPEMD160 hash list
        /// </summary>
        RedumpRIPEMD160,

        /// <summary>
        /// SHA-1 hash list
        /// </summary>
        RedumpSHA1,

        /// <summary>
        /// SHA-256 hash list
        /// </summary>
        RedumpSHA256,

        /// <summary>
        /// SHA-384 hash list
        /// </summary>
        RedumpSHA384,

        /// <summary>
        /// SHA-512 hash list
        /// </summary>
        RedumpSHA512,

        /// <summary>
        /// SpamSum hash list
        /// </summary>
        RedumpSpamSum,

        #endregion
    }
}
