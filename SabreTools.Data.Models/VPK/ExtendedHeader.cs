using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.VPK
{
    /// <summary>
    /// Added in version 2.
    /// </summary>
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/VPKFile.h"/>
    /// <see href="https://developer.valvesoftware.com/wiki/VPK_(file_format)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ExtendedHeader
    {
        /// <summary>
        /// How many bytes of file content are stored in this
        /// VPK file (0 in CSGO)
        /// </summary>
        public uint FileDataSectionSize;

        /// <summary>
        /// The size, in bytes, of the section containing MD5
        /// checksums for external archive content
        /// </summary>
        public uint ArchiveMD5SectionSize;

        /// <summary>
        /// The size, in bytes, of the section containing MD5
        /// checksums for content in this file (should always be 48)
        /// </summary>
        public uint OtherMD5SectionSize;

        /// <summary>
        /// The size, in bytes, of the section containing the public
        /// key and signature. This is either 0 (CSGO & The Ship) or
        /// 296 (HL2, HL2:DM, HL2:EP1, HL2:EP2, HL2:LC, TF2, DOD:S & CS:S)
        /// </summary>
        public uint SignatureSectionSize;
    }
}
