
namespace SabreTools.Serialization.Models.SecuROM
{
    public static class Constants
    {
        #region AddD

        public const string AddDMagicString = "AddD";

        public static readonly byte[] AddDMagicBytes = [0x41, 0x64, 0x64, 0x44];

        #endregion

        #region DFA

        public static readonly string DFAMagicString = "SDFA" + (char)0x04 + (char)0x00 + (char)0x00 + (char)0x00;

        public static readonly byte[] DFAMagicBytes = [0x53, 0x44, 0x46, 0x41, 0x04, 0x00, 0x00, 0x00];

        #region Keys

        /// <summary>
        /// 128-bit value, possibly a GUID
        /// </summary>
        public const string COID = "COID";

        /// <summary>
        /// 128-bit value, possibly a GUID
        /// </summary>
        /// <remarks>Only a value of D0 A2 25 C7 16 20 B7 43 99 74 2A BB 39 6B C3 57 has been found</remarks>
        public const string CUID = "CUID";

        /// <summary>
        /// Encrypted data section
        /// </summary>
        public const string DATA = "DATA";

        /// <summary>
        /// Header version (?)
        /// </summary>
        /// <remarks>Only a value of 0C 00 00 00 has been found</remarks>
        public const string HVER = "HVER";

        /// <summary>
        /// Unknown value
        /// </summary>
        public const string INVE = "INVE";

        /// <summary>
        /// Unknown key value
        /// </summary>
        public const string KEYB = "KEYB";

        /// <summary>
        /// Unknown key value
        /// </summary>
        public const string KEYL = "KEYL";

        /// <summary>
        /// MAC address (?)
        /// </summary>
        public const string MAC1 = "MAC1";

        /// <summary>
        /// MAC address (?)
        /// </summary>
        public const string MAC2 = "MAC2";

        /// <summary>
        /// Padding section
        /// </summary>
        /// <remarks>Only a length of 832 has been found</remarks>
        public const string PAD1 = "PAD1";

        /// <summary>
        /// Private key ID (?)
        /// </summary>
        public const string PKID = "PKID";

        /// <summary>
        /// Private key name (?)
        /// </summary>
        /// <remarks>Seemingly a UTF-16 string</remarks>
        public const string PKNA = "PKNA";

        /// <summary>
        /// Size of the decrypted executable
        /// </summary>
        public const string RAWS = "RAWS";

        /// <summary>
        /// 128-bit value, possibly a GUID
        /// </summary>
        /// <remarks>Only a value of all zeroes has been found</remarks>
        public const string SCID = "SCID";

        /// <summary>
        /// Time stored in NTFS filetime
        /// </summary>
        /// <see href="https://learn.microsoft.com/en-us/windows/win32/sysinfo/file-times"/>
        public const string TIME = "TIME";

        /// <summary>
        /// First URL to connect to
        /// </summary>
        public const string UR01 = "UR01";

        /// <summary>
        /// Second URL to connect to
        /// </summary>
        public const string UR02 = "UR02";

        /// <summary>
        /// Unknown value
        /// </summary>
        public const string XSPF = "XSPF";

        #endregion

        #endregion

        #region Matroshka

        public const string MatroshkaMagicString = "MatR";

        public static readonly byte[] MatroshkaMagicBytes = [0x4D, 0x61, 0x74, 0x52];

        #endregion
    }
}