using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.DVD
{
    /// <see href="https://dvd.sourceforge.net/dvdinfo/ifo.html"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class VideoManagerIFO
    {
        /// <summary>
        /// "DVDVIDEO-VMG"
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string Signature = string.Empty;

        /// <summary>
        /// Last sector of VMG set (last sector of BUP)
        /// </summary>
        public uint LastVMGSetSector;

        /// <summary>
        /// Last sector of IFO
        /// </summary>
        public uint LastIFOSector;

        /// <summary>
        /// Version number
        /// - Byte 0 - Reserved, should be 0
        /// - Byte 1, Bits 7-4 - Major version number
        /// - Byte 1, Bits 3-0 - Minor version number
        /// </summary>
        public ushort VersionNumber;

        /// <summary>
        /// VMG category
        /// </summary>
        /// <remarks>byte1=prohibited region mask</remarks>
        public uint VMGCategory;

        /// <summary>
        /// Number of volumes
        /// </summary>
        public ushort NumberOfVolumes;

        /// <summary>
        /// Volume number
        /// </summary>
        public ushort VolumeNumber;

        /// <summary>
        /// Side ID
        /// </summary>
        public byte SideID;

        /// <summary>
        /// Number of title sets
        /// </summary>
        public ushort NumberOfTitleSets;

        /// <summary>
        /// Provider ID
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] ProviderID = new byte[32];

        /// <summary>
        /// VMG POS
        /// </summary>
        public ulong VMGPOS;

        /// <summary>
        /// End byte address of VMGI_MAT
        /// </summary>
        public uint InformationManagementTableEndByteAddress;

        /// <summary>
        /// Start address of FP_PGC (First Play program chain)
        /// </summary>
        public uint FirstPlayProgramChainStartAddress;

        /// <summary>
        /// Start sector of Menu VOB
        /// </summary>
        public uint MenuVOBStartSector;

        /// <summary>
        /// Sector pointer to TT_SRPT (table of titles)
        /// </summary>
        public uint TableOfTitlesSectorPointer;

        /// <summary>
        /// Sector pointer to VMGM_PGCI_UT (Menu Program Chain table)
        /// </summary>
        public uint MenuProgramChainTableSectorPointer;

        /// <summary>
        /// Sector pointer to VMG_PTL_MAIT (Parental Management masks)
        /// </summary>
        public uint ParentalManagementMasksSectorPointer;

        /// <summary>
        /// Sector pointer to VMG_VTS_ATRT (copies of VTS audio/sub-picture attributes)
        /// </summary>
        public uint AudioSubPictureAttributesSectorPointer;

        /// <summary>
        /// Sector pointer to VMG_TXTDT_MG (text data)
        /// </summary>
        public uint TextDataSectorPointer;

        /// <summary>
        /// Sector pointer to VMGM_C_ADT (menu cell address table)
        /// </summary>
        public uint MenuCellAddressTableSectorPointer;

        /// <summary>
        /// Sector pointer to VMGM_VOBU_ADMAP (menu VOBU address map)
        /// </summary>
        public uint MenuVOBUAddressMapSectorPointer;

        /// <summary>
        /// Video attributes of VMGM_VOBS
        /// </summary>
        public ushort VideoAttributes;

        /// <summary>
        /// Number of audio streams in VMGM_VOBS
        /// </summary>
        public ushort NumberOfAudioStreams;

        /// <summary>
        /// Audio attributes of VMGM_VOBS
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public ulong[] AudioAttributes = new ulong[8];

        /// <summary>
        /// Unknown
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] Unknown = new byte[16];

        /// <summary>
        /// Number of subpicture streams in VMGM_VOBS (0 or 1)
        /// </summary>
        public ushort NumberOfSubpictureStreams;

        /// <summary>
        /// Subpicture attributes of VMGM_VOBS
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] SubpictureAttributes = new byte[6];

        /// <summary>
        /// Reserved
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 164)]
        public byte[] Reserved = new byte[164];
    }
}
