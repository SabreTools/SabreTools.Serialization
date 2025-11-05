namespace SabreTools.Data.Models.CDROM
{
    /// <summary>
    /// CDROM constant values
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public static class Constants
    {
        /// <summary>
        /// Size of a complete CDROM data sector
        /// </summary>
        public const long CDROMSectorSize = 2352;

        /// <summary>
        /// Size of user data length for Mode0 / Mode2 Formless
        /// </summary>
        public const long Mode0DataSize = 2336;

        /// <summary>
        /// Size of user data length for Mode1 / Mode2 Form1
        /// </summary>
        public const long Mode1DataSize = 2048;

        /// <summary>
        /// Size of user data length for Mode1 / Mode2 Form1
        /// </summary>
        public const long Form1DataSize = 2048;

        /// <summary>
        /// Size of user data length for Mode2 Form2
        /// </summary>
        public const long Form2DataSize = 2324;
    }
}
