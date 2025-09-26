using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.NewExecutable
{
    /// <summary>
    /// The NE header is a relatively large structure with multiple characteristics.
    /// Because of the age of the format some items are unclear in meaning.
    /// </summary>
    /// <see href="https://web.archive.org/web/20240422070115/http://bytepointer.com/resources/win16_ne_exe_format_win3.0.htm"/>
    /// <see href="https://github.com/libyal/libexe/blob/main/documentation/Executable%20(EXE)%20file%20format.asciidoc#24-ne-extended-header"/>
    /// <see href="https://wiki.osdev.org/NE"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class ExecutableHeader
    {
        /// <summary>
        /// "NE"
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string? Magic;

        /// <summary>
        /// Version number of the linker.
        /// </summary>
        /// <remarks>Also known as the major linker version</remarks>
        public byte LinkerVersion;

        /// <summary>
        /// Revision number of the linker.
        /// </summary>
        /// <remarks>Also known as the minor linker version</remarks>
        public byte LinkerRevision;

        /// <summary>
        /// Entry Table file offset, relative to the beginning of the segmented EXE header.
        /// </summary>
        public ushort EntryTableOffset;

        /// <summary>
        /// Length of entry table in bytes
        /// </summary>
        public ushort EntryTableSize;

        /// <summary>
        /// 32-bit CRC of entire contents of file.
        /// </summary>
        /// <remarks>These words are taken as 00 during the calculation.</remarks>
        public uint CrcChecksum;

        /// <summary>
        /// Flag word
        /// </summary>
        [MarshalAs(UnmanagedType.U2)]
        public HeaderFlag FlagWord;

        /// <summary>
        /// Segment number of automatic data segment.
        /// This value is set to zero if SINGLEDATA and
        /// MULTIPLEDATA flag bits are clear, NOAUTODATA is
        /// indicated in the flags word.
        /// </summary>
        /// <remarks>
        /// A Segment number is an index into the module's segment
        /// table. The first entry in the segment table is segment
        /// number 1.
        /// </remarks>
        public ushort AutomaticDataSegmentNumber;

        /// <summary>
        /// Initial size, in bytes, of dynamic heap added to the
        /// data segment. This value is zero if no initial local
        /// heap is allocated.
        /// </summary>
        public ushort InitialHeapAlloc;

        /// <summary>
        /// Initial size, in bytes, of stack added to the data
        /// segment. This value is zero to indicate no initial
        /// stack allocation, or when SS is not equal to DS.
        /// </summary>
        public ushort InitialStackAlloc;

        /// <summary>
        /// Segment number:offset of CS:IP.
        /// </summary>
        /// <remarks>CS:IP entry point, CS is index into segment table</remarks>
        public uint InitialCSIPSetting;

        /// <summary>
        /// Segment number:offset of SS:SP.
        /// </summary>
        /// <remarks>
        /// SS:SP initial stack pointer, SS is index into segment table
        /// 
        /// If SS equals the automatic data segment and SP equals
        /// zero, the stack pointer is set to the top of the
        /// automatic data segment just below the additional heap
        /// area.
        /// </remarks>
        public uint InitialSSSPSetting;

        /// <summary>
        /// Number of entries in the Segment Table.
        /// </summary>
        public ushort FileSegmentCount;

        /// <summary>
        /// Number of entries in the Module Reference Table.
        /// </summary>
        public ushort ModuleReferenceTableSize;

        /// <summary>
        /// Size of non-resident names table in bytes
        /// </summary>
        public ushort NonResidentNameTableSize;

        /// <summary>
        /// Segment Table file offset, relative to the beginning
        /// of the segmented EXE header.
        /// </summary>
        public ushort SegmentTableOffset;

        /// <summary>
        /// Resource Table file offset, relative to the beginning
        /// of the segmented EXE header.
        /// </summary>
        public ushort ResourceTableOffset;

        /// <summary>
        /// Resident Name Table file offset, relative to the
        /// beginning of the segmented EXE header.
        /// </summary>
        public ushort ResidentNameTableOffset;

        /// <summary>
        /// Module Reference Table file offset, relative to the
        /// beginning of the segmented EXE header.
        /// </summary>
        public ushort ModuleReferenceTableOffset;

        /// <summary>
        /// Imported Names Table file offset, relative to the
        /// beginning of the segmented EXE header.
        /// </summary>
        public ushort ImportedNamesTableOffset;

        /// <summary>
        /// Non-Resident Name Table offset, relative to the
        /// beginning of the file.
        /// </summary>
        public uint NonResidentNamesTableOffset;

        /// <summary>
        /// Number of movable entries in the Entry Table.
        /// </summary>
        public ushort MovableEntriesCount;

        /// <summary>
        /// Logical sector alignment shift count, log(base 2) of
        /// the segment sector size (default 9).
        /// </summary>
        public ushort SegmentAlignmentShiftCount;

        /// <summary>
        /// Number of resource entries.
        /// </summary>
        public ushort ResourceEntriesCount;

        /// <summary>
        /// Executable type, used by loader.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public OperatingSystem TargetOperatingSystem;

        #region OS/2 Specific

        /// <summary>
        /// Other OS/2 flags
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public OS2Flag AdditionalFlags;

        /// <summary>
        /// Offset to return thunks or start of gangload area
        /// </summary>
        public ushort ReturnThunkOffset;

        /// <summary>
        /// Offset to segment reference thunks or size of gangload area
        /// </summary>
        public ushort SegmentReferenceThunkOffset;

        /// <summary>
        /// Minimum code swap area size
        /// </summary>
        public ushort MinCodeSwapAreaSize;

        /// <summary>
        /// Windows SDK revison number
        /// </summary>
        public byte WindowsSDKRevision;

        /// <summary>
        /// Windows SDK version number
        /// </summary>
        public byte WindowsSDKVersion;

        #endregion
    }
}
