namespace SabreTools.Data.Models.ISO9660
{
    /// <summary>
    /// ISO9660 constant values and arrays
    /// </summary>
    /// <see href="https://ecma-international.org/wp-content/uploads/ECMA-119_5th_edition_december_2024.pdf"/>
    public static class Constants
    {
        #region Volume Descriptor Constants

        /// <summary>
        /// Minimum size of a logical sector
        /// </summary>
        public const int MinimumSectorSize = 2048;

        /// <summary>
        /// Number of logical sectors in the System Area
        /// </summary>
        public const int SystemAreaSectors = 16;

        /// <summary>
        /// Identifier used for ISO9660, "CD001"
        /// </summary>
        public static readonly byte[] StandardIdentifier = [0x43, 0x44, 0x30, 0x30, 0x31];

        /// <summary>
        /// File Identifier of the current directory
        /// </summary>
        public static readonly byte[] CurrentDirectory = [0x00];

        /// <summary>
        /// File Identifier of the parent directory
        /// </summary>
        public static readonly byte[] ParentDirectory = [0x01];

        #endregion

        #region CD-i Constants

        /// <summary>
        /// Identifier present on non-ISO9660 CD-i discs, "CD-I "
        /// </summary>
        public static readonly byte[] StandardIdentifierCDI = [0x43, 0x44, 0x2D, 0x49, 0x20];

        #endregion

        #region High Sierra Constants

        /// <summary>
        /// Identifier present on High Sierra discs, "CDROM"
        /// </summary>
        /// <remarks>
        /// Recorded at bytes 9-13 of a High Sierra volume descriptor,
        /// after its logical block number (bytes 0-7) and type (byte 8)
        /// </remarks>
        public static readonly byte[] HighSierraIdentifier = [0x43, 0x44, 0x52, 0x4F, 0x4D];

        #endregion

        #region Primary/Supplementary Volume Descriptors Constants

        /// <summary>
        /// Character used for separating a file name from a file extension, Fullstop character "."
        /// This value is used in Primary Volume Descriptors
        /// </summary>
        public const byte Separator1 = 0x2E;

        /// <summary>
        /// Character used for separating the file name/extension, from the file version number, Semicolon character ";"
        /// This value is used in Primary Volume Descriptors
        /// </summary>
        public const byte Separator2 = 0x3B;

        /// <summary>
        /// Character used for padding a byte array on the right, Space character " "
        /// This value is used in Primary/Supplementary Volume Descriptors
        /// </summary>
        public const byte Filler = 0x20;

        /// <summary>
        /// Valid a-characters: A subset of 57 ASCII characters including A-Z, 0-9, and some special characters (0x20-0x22, 0x25-0x3F, 0x41-0x5A, 0x5F)
        /// A B C D E F G H I J K L M N O P Q R S T U V W X Y Z 0 1 2 3 4 5 6 7 8 9 _ ! " % & ' ( ) * + , - . / : ; < = > ?
        /// Note: a1-characters are a user-defined subset of c-characters (UCS-2)
        /// Note: Joliet extension implies all MSB UCS-2 characters except control characters and * / : ; ? \ (0x0000-0x001F, 0x002A, 0x002F, 0x003A, 0x003B, 0x003F, 0x005C)
        /// </summary>
        public static readonly byte[] ValidACharacters =
        [
            0x20, 0x21, 0x22, 0x25, 0x26, 0x27, 0x28, 0x29,
            0x2A, 0x2B, 0x2C, 0x2D, 0x2E, 0x2F, 0x30, 0x31,
            0x32, 0x33, 0x34, 0x35, 0x36, 0x37, 0x38, 0x39,
            0x3A, 0x3B, 0x3C, 0x3D, 0x3E, 0x3F, 0x41, 0x42,
            0x43, 0x44, 0x45, 0x46, 0x47, 0x48, 0x49, 0x4A,
            0x4B, 0x4C, 0x4D, 0x4E, 0x4F, 0x50, 0x51, 0x52,
            0x53, 0x54, 0x55, 0x56, 0x57, 0x58, 0x59, 0x5A,
            0x5F
        ];

        /// <summary>
        /// Valid d-characters: A subset of 37 ASCII characters including A-Z, 0-9, and underscore only (0x30-0x3F, 0x41-0x5A, 0x5F)
        /// A B C D E F G H I J K L M N O P Q R S T U V W X Y Z 0 1 2 3 4 5 6 7 8 9 _
        /// Note: d1-characters are a user-defined subset of c-characters (UCS-2)
        /// Note: Joliet extension implies all MSB UCS-2 characters except control characters and * / : ; ? \ (0x0000-0x001F, 0x002A, 0x002F, 0x003A, 0x003B, 0x003F, 0x005C)
        /// </summary>
        public static readonly byte[] ValidDCharacters =
        [
            0x30, 0x31, 0x32, 0x33, 0x34, 0x35, 0x36, 0x37,
            0x38, 0x39, 0x41, 0x42, 0x43, 0x44, 0x45, 0x46,
            0x47, 0x48, 0x49, 0x4A, 0x4B, 0x4C, 0x4D, 0x4E,
            0x4F, 0x50, 0x51, 0x52, 0x53, 0x54, 0x55, 0x56,
            0x57, 0x78, 0x59, 0x5A, 0x5F
        ];

        #endregion

        #region Joliet Extension Constants

        /// <summary>
        /// UCS-2 Character used for separating a file name from a file extension, Fullstop character "."
        /// This is used in Joliet-style Enhanced Volume Descriptors
        /// </summary>
        public static readonly byte[] JolietSeparator1 = [0x00, 0x2E];

        /// <summary>
        /// UCS-2 Character used for separating the file name/extension, from the file version number, Semicolon character ";"
        /// This is used in Joliet-style Enhanced Volume Descriptors
        /// </summary>
        public static readonly byte[] JolietSeparator2 = [0x00, 0x3B];

        /// <summary>
        /// Character used for padding a byte array on the right, null character
        /// This is used in Joliet-style Enhanced Volume Descriptors
        /// </summary>
        public const byte JolietFiller = 0x20;

        /// <summary>
        /// Joliet extension uses Enhanced Volume Descriptor with this VolumeFlags value
        /// </summary>
        public const byte JolietVolumeFlags = 0x00;

        /// <summary>
        /// Joliet extension uses Enhanced Volume Descriptor with this EscapeSequences value
        /// Escape Sequences: (25 2F 40) (25 2F 43) (25 2F 45)
        /// </summary>
        public static readonly byte[] JolietEscapeSequences =
        [
            0x25, 0x2F, 0x40, 0x25, 0x2F, 0x43, 0x25, 0x2F,
            0x45, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        ];

        #endregion

        #region System Use Sharing Protocol Constants

        /// <summary>
        /// Signature of a System Use Sharing Protocol (SUSP) Continuation Area entry, "CE"
        /// </summary>
        public const string ContinuationAreaSignature = "CE";

        /// <summary>
        /// Signature of a System Use Sharing Protocol (SUSP) Padding Field entry, "PD"
        /// </summary>
        public const string PaddingFieldSignature = "PD";

        /// <summary>
        /// Signature of a System Use Sharing Protocol (SUSP) Sharing Protocol Indicator entry, "SP"
        /// </summary>
        public const string SharingProtocolIndicatorSignature = "SP";

        /// <summary>
        /// Signature of a System Use Sharing Protocol (SUSP) Sharing Protocol Terminator entry, "ST"
        /// </summary>
        public const string SharingProtocolTerminatorSignature = "ST";

        /// <summary>
        /// Signature of a System Use Sharing Protocol (SUSP) Extensions Reference entry, "ER"
        /// </summary>
        public const string ExtensionsReferenceSignature = "ER";

        /// <summary>
        /// Signature of a System Use Sharing Protocol (SUSP) Extension Selector entry, "ES"
        /// </summary>
        public const string ExtensionSelectorSignature = "ES";

        /// <summary>
        /// Check bytes of a Sharing Protocol Indicator entry, which tell it from other data
        /// </summary>
        public static readonly byte[] SharingProtocolCheckBytes = [0xBE, 0xEF];

        #endregion

        #region Rock Ridge Constants

        /// <summary>
        /// Signature of a Rock Ridge POSIX file attributes entry, "PX"
        /// </summary>
        public const string PosixFileAttributesSignature = "PX";

        /// <summary>
        /// Signature of a Rock Ridge POSIX device number entry, "PN"
        /// </summary>
        public const string PosixDeviceNumberSignature = "PN";

        /// <summary>
        /// Signature of a Rock Ridge symbolic link entry, "SL"
        /// </summary>
        public const string SymbolicLinkSignature = "SL";

        /// <summary>
        /// Signature of a Rock Ridge alternate name entry, "NM"
        /// </summary>
        public const string AlternateNameSignature = "NM";

        /// <summary>
        /// Signature of a Rock Ridge child link entry, "CL"
        /// </summary>
        public const string ChildLinkSignature = "CL";

        /// <summary>
        /// Signature of a Rock Ridge parent link entry, "PL"
        /// </summary>
        public const string ParentLinkSignature = "PL";

        /// <summary>
        /// Signature of a Rock Ridge relocated directory entry, "RE"
        /// </summary>
        public const string RelocatedDirectorySignature = "RE";

        /// <summary>
        /// Signature of a Rock Ridge time stamps entry, "TF"
        /// </summary>
        public const string TimeStampsSignature = "TF";

        /// <summary>
        /// Signature of a Rock Ridge sparse file entry, "SF"
        /// </summary>
        public const string SparseFileSignature = "SF";

        /// <summary>
        /// Signature of a Rock Ridge extensions in use entry, "RR"
        /// </summary>
        public const string ExtensionsInUseSignature = "RR";

        #endregion

        #region Apple Extension Constants

        /// <summary>
        /// Signature of the Apple extension entry for HFS, which holds the Finder type, creator and flags of a file, "AA"
        /// </summary>
        public const string AppleExtensionSignature = "AA";

        #endregion
    }
}
