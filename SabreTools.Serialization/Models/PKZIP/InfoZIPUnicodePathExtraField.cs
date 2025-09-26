namespace SabreTools.Data.Models.PKZIP
{
    /// <summary>
    /// Stores the UTF-8 version of the file name field as stored in the
    /// local header and central directory header. (Last Revision 20070912)
    /// 
    /// Currently Version is set to the number 1.  If there is a need
    /// to change this field, the version will be incremented.  Changes
    /// MAY NOT be backward compatible so this extra field SHOULD NOT be
    /// used if the version is not recognized.
    /// 
    /// The NameCRC32 is the standard zip CRC32 checksum of the File Name
    /// field in the header.  This is used to verify that the header
    /// File Name field has not changed since the Unicode Path extra field
    /// was created.  This can happen if a utility renames the File Name but
    /// does not update the UTF-8 path extra field.  If the CRC check fails,
    /// this UTF-8 Path Extra Field SHOULD be ignored and the File Name field
    /// in the header SHOULD be used instead.
    /// 
    /// The UnicodeName is the UTF-8 version of the contents of the File Name
    /// field in the header.  As UnicodeName is defined to be UTF-8, no UTF-8
    /// byte order mark (BOM) is used.  The length of this field is determined
    /// by subtracting the size of the previous fields from TSize.  If both
    /// the File Name and Comment fields are UTF-8, the new General Purpose
    /// Bit Flag, bit 11 (Language encoding flag (EFS)), can be used to
    /// indicate that both the header File Name and Comment fields are UTF-8
    /// and, in this case, the Unicode Path and Unicode Comment extra fields
    /// are not needed and SHOULD NOT be created.  Note that, for backward
    /// compatibility, bit 11 SHOULD only be used if the native character set
    /// of the paths and comments being zipped up are already in UTF-8. It is
    /// expected that the same file name storage method, either general
    /// purpose bit 11 or extra fields, be used in both the Local and Central
    /// Directory Header for a file.
    /// </summary>
    /// <remarks>Header ID = 0x7075</remarks>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/> 
    public class InfoZIPUnicodePathExtraField : ExtensibleDataField
    {
        /// <summary>
        /// Version of this extra field, currently 1
        /// </summary>
        public byte Version { get; set; }

        /// <summary>
        /// File Name Field CRC32 Checksum
        /// </summary>
        public uint NameCRC32 { get; set; }

        /// <summary>
        /// UTF-8 version of the entry File Name
        /// </summary>
        public string? UnicodeName { get; set; }
    }
}
