namespace SabreTools.Data.Models.CDROM
{
    /// <summary>
    /// CDROM constant values
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-130_2nd_edition_june_1996.pdf"/>
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
        /// Offset in a Mode0 sector where user data starts
        /// </summary>
        public const long Mode0UserDataStart = 16;

        /// <summary>
        /// Offset in a Mode0 sector where user data ends
        /// </summary>
        public const long Mode0UserDataEnd = 2064;

        /// <summary>
        /// Size of user data length for Mode1
        /// </summary>
        public const long Mode1DataSize = 2048;

        /// <summary>
        /// Offset in a Mode1 sector where user data starts
        /// </summary>
        public const long Mode1UserDataStart = 16;

        /// <summary>
        /// Offset in a Mode1 sector where user data ends
        /// </summary>
        public const long Mode1UserDataEnd = 2064;

        /// <summary>
        /// Size of user data length for Mode2 Form1
        /// </summary>
        public const long Mode2Form1DataSize = 2048;

        /// <summary>
        /// Offset in a Mode2 Form1 sector where user data starts
        /// </summary>
        public const long Mode2Form1UserDataStart = 24;

        /// <summary>
        /// Offset in a Mode2 Form1 sector where user data ends
        /// </summary>
        public const long Mode2Form1UserDataEnd = 2072;

        /// <summary>
        /// Size of user data length for Mode2 Form2
        /// </summary>
        public const long Mode2Form2DataSize = 2324;

        /// <summary>
        /// Offset in a Mode2 Form2 sector where user data starts
        /// </summary>
        public const long Mode2Form2UserDataStart = 24;

        /// <summary>
        /// Offset in a Mode2 Form2 sector where user data ends
        /// </summary>
        public const long Mode2Form2UserDataEnd = 2072;
    }
}
