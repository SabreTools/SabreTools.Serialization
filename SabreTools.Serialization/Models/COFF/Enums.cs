using System;

namespace SabreTools.Data.Models.COFF
{
    [Flags]
    public enum Characteristics : ushort
    {
        /// <summary>
        /// Image only, Windows CE, and Microsoft Windows NT and later.
        /// This indicates that the file does not contain base relocations
        /// and must therefore be loaded at its preferred base address.
        /// If the base address is not available, the loader reports an
        /// error. The default behavior of the linker is to strip base
        /// relocations from executable (EXE) files.
        /// </summary>
        IMAGE_FILE_RELOCS_STRIPPED = 0x0001,

        /// <summary>
        /// Image only. This indicates that the image file is valid and
        /// can be run. If this flag is not set, it indicates a linker error.
        /// </summary>
        IMAGE_FILE_EXECUTABLE_IMAGE = 0x0002,

        /// <summary>
        /// COFF line numbers have been removed. This flag is deprecated
        /// and should be zero.
        /// </summary>
        IMAGE_FILE_LINE_NUMS_STRIPPED = 0x0004,

        /// <summary>
        /// COFF symbol table entries for local symbols have been removed.
        /// This flag is deprecated and should be zero.
        /// </summary>
        IMAGE_FILE_LOCAL_SYMS_STRIPPED = 0x0008,

        /// <summary>
        /// Obsolete. Aggressively trim working set. This flag is deprecated
        /// for Windows 2000 and later and must be zero.
        /// </summary>
        IMAGE_FILE_AGGRESSIVE_WS_TRIM = 0x0010,

        /// <summary>
        /// Application can handle > 2-GB addresses.
        /// </summary>
        IMAGE_FILE_LARGE_ADDRESS_AWARE = 0x0020,

        /// <summary>
        /// This flag is reserved for future use.
        /// </summary>
        RESERVED = 0x0040,

        /// <summary>
        /// Little endian: the least significant bit (LSB) precedes the most
        /// significant bit (MSB) in memory. This flag is deprecated and
        /// should be zero.
        /// </summary>
        IMAGE_FILE_BYTES_REVERSED_LO = 0x0080,

        /// <summary>
        /// Machine is based on a 32-bit-word architecture.
        /// </summary>
        IMAGE_FILE_32BIT_MACHINE = 0x0100,

        /// <summary>
        /// Debugging information is removed from the image file.
        /// </summary>
        IMAGE_FILE_DEBUG_STRIPPED = 0x0200,

        /// <summary>
        /// If the image is on removable media, fully load it and
        /// copy it to the swap file.
        /// </summary>
        IMAGE_FILE_REMOVABLE_RUN_FROM_SWAP = 0x0400,

        /// <summary>
        /// If the image is on network media, fully load it and copy
        /// it to the swap file.
        /// </summary>
        IMAGE_FILE_NET_RUN_FROM_SWAP = 0x0800,

        /// <summary>
        /// The image file is a system file, not a user program.
        /// </summary>
        IMAGE_FILE_SYSTEM = 0x1000,

        /// <summary>
        /// The image file is a dynamic-link library (DLL). Such files
        /// are considered executable files for almost all purposes,
        /// although they cannot be directly run.
        /// </summary>
        IMAGE_FILE_DLL = 0x2000,

        /// <summary>
        /// The file should be run only on a uniprocessor machine.
        /// </summary>
        IMAGE_FILE_UP_SYSTEM_ONLY = 0x4000,

        /// <summary>
        /// Big endian: the MSB precedes the LSB in memory. This flag
        /// is deprecated and should be zero.
        /// </summary>
        IMAGE_FILE_BYTES_REVERSED_HI = 0x8000,
    }

    public enum MachineType : ushort
    {
        /// <summary>
        /// The content of this field is assumed to be applicable to any machine type
        /// </summary>
        IMAGE_FILE_MACHINE_UNKNOWN = 0x0000,

        /// <summary>
        /// Matsushita AM33
        /// </summary>
        IMAGE_FILE_MACHINE_AM33 = 0x01D3,

        /// <summary>
        /// x64
        /// </summary>
        IMAGE_FILE_MACHINE_AMD64 = 0x8664,

        /// <summary>
        /// ARM little endian
        /// </summary>
        IMAGE_FILE_MACHINE_ARM = 0x01C0,

        /// <summary>
        /// ARM64 little endian
        /// </summary>
        IMAGE_FILE_MACHINE_ARM64 = 0xAA64,

        /// <summary>
        /// ARM Thumb-2 little endian
        /// </summary>
        IMAGE_FILE_MACHINE_ARMNT = 0x01C4,

        /// <summary>
        /// EFI byte code
        /// </summary>
        IMAGE_FILE_MACHINE_EBC = 0x0EBC,

        /// <summary>
        /// Intel 386 or later processors and compatible processors
        /// </summary>
        IMAGE_FILE_MACHINE_I386 = 0x014C,

        /// <summary>
        /// Intel Itanium processor family
        /// </summary>
        IMAGE_FILE_MACHINE_IA64 = 0x0200,

        /// <summary>
        /// LoongArch 32-bit processor family
        /// </summary>
        IMAGE_FILE_MACHINE_LOONGARCH32 = 0x6232,

        /// <summary>
        /// LoongArch 64-bit processor family
        /// </summary>
        IMAGE_FILE_MACHINE_LOONGARCH64 = 0x6264,

        /// <summary>
        /// Mitsubishi M32R little endian
        /// </summary>
        IMAGE_FILE_MACHINE_M32R = 0x9041,

        /// <summary>
        /// MIPS16
        /// </summary>
        IMAGE_FILE_MACHINE_MIPS16 = 0x0266,

        /// <summary>
        /// MIPS with FPU
        /// </summary>
        IMAGE_FILE_MACHINE_MIPSFPU = 0x0366,

        /// <summary>
        /// MIPS16 with FPU
        /// </summary>
        IMAGE_FILE_MACHINE_MIPSFPU16 = 0x0466,

        /// <summary>
        /// Power PC little endian
        /// </summary>
        IMAGE_FILE_MACHINE_POWERPC = 0x01F0,

        /// <summary>
        /// Power PC with floating point support
        /// </summary>
        IMAGE_FILE_MACHINE_POWERPCFP = 0x01F1,

        /// <summary>
        /// MIPS little endian
        /// </summary>
        IMAGE_FILE_MACHINE_R4000 = 0x0166,

        /// <summary>
        /// RISC-V 32-bit address space
        /// </summary>
        IMAGE_FILE_MACHINE_RISCV32 = 0x5032,

        /// <summary>
        /// RISC-V 64-bit address space
        /// </summary>
        IMAGE_FILE_MACHINE_RISCV64 = 0x5064,

        /// <summary>
        /// RISC-V 128-bit address space
        /// </summary>
        IMAGE_FILE_MACHINE_RISCV128 = 0x5128,

        /// <summary>
        /// Hitachi SH3
        /// </summary>
        IMAGE_FILE_MACHINE_SH3 = 0x01A2,

        /// <summary>
        /// Hitachi SH3 DSP
        /// </summary>
        IMAGE_FILE_MACHINE_SH3DSP = 0x01A3,

        /// <summary>
        /// Hitachi SH4
        /// </summary>
        IMAGE_FILE_MACHINE_SH4 = 0x01A6,

        /// <summary>
        /// Hitachi SH5
        /// </summary>
        IMAGE_FILE_MACHINE_SH5 = 0x01A8,

        /// <summary>
        /// Thumb
        /// </summary>
        IMAGE_FILE_MACHINE_THUMB = 0x01C2,

        /// <summary>
        /// MIPS little-endian WCE v2
        /// </summary>
        IMAGE_FILE_MACHINE_WCEMIPSV2 = 0x0169,
    }

    public enum OptionalHeaderMagicNumber : ushort
    {
        ROMImage = 0x0107,

        PE32 = 0x010B,

        PE32Plus = 0x020B,
    }

    public enum RelocationType : ushort
    {
        #region x64 Processors

        /// <summary>
        /// The relocation is ignored.
        /// </summary>
        IMAGE_REL_AMD64_ABSOLUTE = 0x0000,

        /// <summary>
        /// The 64-bit VA of the relocation target.
        /// </summary>
        IMAGE_REL_AMD64_ADDR64 = 0x0001,

        /// <summary>
        /// The 32-bit VA of the relocation target.
        /// </summary>
        IMAGE_REL_AMD64_ADDR32 = 0x0002,

        /// <summary>
        /// The 32-bit address without an image base (RVA).
        /// </summary>
        IMAGE_REL_AMD64_ADDR32NB = 0x0003,

        /// <summary>
        /// The 32-bit relative address from the byte following the relocation.
        /// </summary>
        IMAGE_REL_AMD64_REL32 = 0x0004,

        /// <summary>
        /// The 32-bit address relative to byte distance 1 from the relocation.
        /// </summary>
        IMAGE_REL_AMD64_REL32_1 = 0x0005,

        /// <summary>
        /// The 32-bit address relative to byte distance 2 from the relocation.
        /// </summary>
        IMAGE_REL_AMD64_REL32_2 = 0x0006,

        /// <summary>
        /// The 32-bit address relative to byte distance 3 from the relocation.
        /// </summary>
        IMAGE_REL_AMD64_REL32_3 = 0x0007,

        /// <summary>
        /// The 32-bit address relative to byte distance 4 from the relocation.
        /// </summary>
        IMAGE_REL_AMD64_REL32_4 = 0x0008,

        /// <summary>
        /// The 32-bit address relative to byte distance 5 from the relocation.
        /// </summary>
        IMAGE_REL_AMD64_REL32_5 = 0x0009,

        /// <summary>
        /// The 16-bit section index of the section that contains the target.
        /// This is used to support debugging information.
        /// </summary>
        IMAGE_REL_AMD64_SECTION = 0x000A,

        /// <summary>
        /// The 32-bit offset of the target from the beginning of its section.
        /// This is used to support debugging information and static thread
        /// local storage.
        /// </summary>
        IMAGE_REL_AMD64_SECREL = 0x000B,

        /// <summary>
        /// A 7-bit unsigned offset from the base of the section that contains
        /// the target.
        /// </summary>
        IMAGE_REL_AMD64_SECREL7 = 0x000C,

        /// <summary>
        /// CLR tokens.
        /// </summary>
        IMAGE_REL_AMD64_TOKEN = 0x000D,

        /// <summary>
        /// A 32-bit signed span-dependent value emitted into the object.
        /// </summary>
        IMAGE_REL_AMD64_SREL32 = 0x000E,

        /// <summary>
        /// A pair that must immediately follow every span-dependent value.
        /// </summary>
        IMAGE_REL_AMD64_PAIR = 0x000F,

        /// <summary>
        /// A 32-bit signed span-dependent value that is applied at link time.
        /// </summary>
        IMAGE_REL_AMD64_SSPAN32 = 0x0010,

        #endregion

        #region ARM Processors

        /// <summary>
        /// The relocation is ignored.
        /// </summary>
        IMAGE_REL_ARM_ABSOLUTE = 0x0000,

        /// <summary>
        /// The 32-bit VA of the target.
        /// </summary>
        IMAGE_REL_ARM_ADDR32 = 0x0001,

        /// <summary>
        /// The 32-bit RVA of the target.
        /// </summary>
        IMAGE_REL_ARM_ADDR32NB = 0x0002,

        /// <summary>
        /// The 24-bit relative displacement to the target.
        /// </summary>
        IMAGE_REL_ARM_BRANCH24 = 0x0003,

        /// <summary>
        /// The reference to a subroutine call. The reference
        /// consists of two 16-bit instructions with 11-bit offsets.
        /// </summary>
        IMAGE_REL_ARM_BRANCH11 = 0x0004,

        /// <summary>
        /// The 32-bit relative address from the byte following the relocation.
        /// </summary>
        IMAGE_REL_ARM_REL32 = 0x000A,

        /// <summary>
        /// The 16-bit section index of the section that contains the target.
        /// This is used to support debugging information.
        /// </summary>
        IMAGE_REL_ARM_SECTION = 0x000E,

        /// <summary>
        /// The 32-bit offset of the target from the beginning of its section.
        /// This is used to support debugging information and static thread
        /// local storage.
        /// </summary>
        IMAGE_REL_ARM_SECREL = 0x000F,

        /// <summary>
        /// The 32-bit VA of the target.This relocation is applied using a MOVW
        /// instruction for the low 16 bits followed by a MOVT for the high 16 bits.
        /// </summary>
        IMAGE_REL_ARM_MOV32 = 0x0010,

        /// <summary>
        /// The 32-bit VA of the target.This relocation is applied using a MOVW
        /// instruction for the low 16 bits followed by a MOVT for the high 16 bits.
        /// </summary>
        IMAGE_REL_THUMB_MOV32 = 0x0011,

        /// <summary>
        /// The instruction is fixed up with the 21 - bit relative displacement to
        /// the 2-byte aligned target. The least significant bit of the displacement
        /// is always zero and is not stored. This relocation corresponds to a
        /// Thumb-2 32-bit conditional B instruction.
        /// </summary>
        IMAGE_REL_THUMB_BRANCH20 = 0x0012,

        Unused = 0x0013,

        /// <summary>
        /// The instruction is fixed up with the 25-bit relative displacement to
        /// the 2-byte aligned target. The least significant bit of the displacement
        /// is zero and is not stored. This relocation corresponds to a Thumb-2 B
        /// instruction.
        /// </summary>
        IMAGE_REL_THUMB_BRANCH24 = 0x0014,

        /// <summary>
        /// The instruction is fixed up with the 25-bit relative displacement to
        /// the 4-byte aligned target. The low 2 bits of the displacement are zero
        /// and are not stored. This relocation corresponds to a Thumb-2 BLX instruction.
        /// </summary>
        IMAGE_REL_THUMB_BLX23 = 0x0015,

        /// <summary>
        /// The relocation is valid only when it immediately follows a ARM_REFHI or
        /// THUMB_REFHI. Its SymbolTableIndex contains a displacement and not an index
        /// into the symbol table.
        /// </summary>
        IMAGE_REL_ARM_PAIR = 0x0016,

        #endregion

        #region ARM64 Processors

        /// <summary>
        /// The relocation is ignored.
        /// </summary>
        IMAGE_REL_ARM64_ABSOLUTE = 0x0000,

        /// <summary>
        /// The 32-bit VA of the target.
        /// </summary>
        IMAGE_REL_ARM64_ADDR32 = 0x0001,

        /// <summary>
        /// The 32-bit RVA of the target.
        /// </summary>
        IMAGE_REL_ARM64_ADDR32NB = 0x0002,

        /// <summary>
        /// The 26-bit relative displacement to the target, for B and BL instructions.
        /// </summary>
        IMAGE_REL_ARM64_BRANCH26 = 0x0003,

        /// <summary>
        /// The page base of the target, for ADRP instruction.
        /// </summary>
        IMAGE_REL_ARM64_PAGEBASE_REL21 = 0x0004,

        /// <summary>
        /// The 12-bit relative displacement to the target, for instruction ADR
        /// </summary>
        IMAGE_REL_ARM64_REL21 = 0x0005,

        /// <summary>
        /// The 12-bit page offset of the target, for instructions ADD/ADDS (immediate)
        /// with zero shift.
        /// </summary>
        IMAGE_REL_ARM64_PAGEOFFSET_12A = 0x0006,

        /// <summary>
        /// The 12-bit page offset of the target, for instruction LDR (indexed,
        /// unsigned immediate).
        /// </summary>
        IMAGE_REL_ARM64_PAGEOFFSET_12L = 0x0007,

        /// <summary>
        /// The 32-bit offset of the target from the beginning of its section.
        /// This is used to support debugging information and static thread local storage.
        /// </summary>
        IMAGE_REL_ARM64_SECREL = 0x0008,

        /// <summary>
        /// Bit 0:11 of section offset of the target, for instructions ADD/ADDS(immediate)
        /// with zero shift.
        /// </summary>
        IMAGE_REL_ARM64_SECREL_LOW12A = 0x0009,

        /// <summary>
        /// Bit 12:23 of section offset of the target, for instructions ADD/ADDS(immediate)
        /// with zero shift.
        /// </summary>
        IMAGE_REL_ARM64_SECREL_HIGH12A = 0x000A,

        /// <summary>
        /// Bit 0:11 of section offset of the target, for instruction LDR(indexed,
        /// unsigned immediate).
        /// </summary>
        IMAGE_REL_ARM64_SECREL_LOW12L = 0x000B,

        /// <summary>
        /// CLR token.
        /// </summary>
        IMAGE_REL_ARM64_TOKEN = 0x000C,

        /// <summary>
        /// The 16-bit section index of the section that contains the target.
        /// This is used to support debugging information.
        /// </summary>
        IMAGE_REL_ARM64_SECTION = 0x000D,

        /// <summary>
        /// The 64-bit VA of the relocation target.
        /// </summary>
        IMAGE_REL_ARM64_ADDR64 = 0x000E,

        /// <summary>
        /// The 19-bit offset to the relocation target, for conditional B instruction.
        /// </summary>
        IMAGE_REL_ARM64_BRANCH19 = 0x000F,

        /// <summary>
        /// The 14-bit offset to the relocation target, for instructions TBZ and TBNZ.
        /// </summary>
        IMAGE_REL_ARM64_BRANCH14 = 0x0010,

        /// <summary>
        /// The 32-bit relative address from the byte following the relocation.
        /// </summary>
        IMAGE_REL_ARM64_REL32 = 0x0011,

        #endregion

        #region Hitachi SuperH Processors

        /// <summary>
        /// The relocation is ignored.
        /// </summary>
        IMAGE_REL_SH3_ABSOLUTE = 0x0000,

        /// <summary>
        /// A reference to the 16-bit location that contains the VA of
        /// the target symbol.
        /// </summary>
        IMAGE_REL_SH3_DIRECT16 = 0x0001,

        /// <summary>
        /// The 32-bit VA of the target symbol.
        /// </summary>
        IMAGE_REL_SH3_DIRECT32 = 0x0002,

        /// <summary>
        /// A reference to the 8-bit location that contains the VA of
        /// the target symbol.
        /// </summary>
        IMAGE_REL_SH3_DIRECT8 = 0x0003,

        /// <summary>
        /// A reference to the 8-bit instruction that contains the
        /// effective 16-bit VA of the target symbol.
        /// </summary>
        IMAGE_REL_SH3_DIRECT8_WORD = 0x0004,

        /// <summary>
        /// A reference to the 8-bit instruction that contains the
        /// effective 32-bit VA of the target symbol.
        /// </summary>
        IMAGE_REL_SH3_DIRECT8_LONG = 0x0005,

        /// <summary>
        /// A reference to the 8-bit location whose low 4 bits contain
        /// the VA of the target symbol.
        /// </summary>
        IMAGE_REL_SH3_DIRECT4 = 0x0006,

        /// <summary>
        /// A reference to the 8-bit instruction whose low 4 bits contain
        /// the effective 16-bit VA of the target symbol.
        /// </summary>
        IMAGE_REL_SH3_DIRECT4_WORD = 0x0007,

        /// <summary>
        /// A reference to the 8-bit instruction whose low 4 bits contain
        /// the effective 32-bit VA of the target symbol.
        /// </summary>
        IMAGE_REL_SH3_DIRECT4_LONG = 0x0008,

        /// <summary>
        /// A reference to the 8-bit instruction that contains the
        /// effective 16-bit relative offset of the target symbol.
        /// </summary>
        IMAGE_REL_SH3_PCREL8_WORD = 0x0009,

        /// <summary>
        /// A reference to the 8-bit instruction that contains the
        /// effective 32-bit relative offset of the target symbol.
        /// </summary>
        IMAGE_REL_SH3_PCREL8_LONG = 0x000A,

        /// <summary>
        /// A reference to the 16-bit instruction whose low 12 bits contain
        /// the effective 16-bit relative offset of the target symbol.
        /// </summary>
        IMAGE_REL_SH3_PCREL12_WORD = 0x000B,

        /// <summary>
        /// A reference to a 32-bit location that is the VA of the
        /// section that contains the target symbol.
        /// </summary>
        IMAGE_REL_SH3_STARTOF_SECTION = 0x000C,

        /// <summary>
        /// A reference to the 32-bit location that is the size of the
        /// section that contains the target symbol.
        /// </summary>
        IMAGE_REL_SH3_SIZEOF_SECTION = 0x000D,

        /// <summary>
        /// The 16-bit section index of the section that contains the target.
        /// This is used to support debugging information.
        /// </summary>
        IMAGE_REL_SH3_SECTION = 0x000E,

        /// <summary>
        /// The 32-bit offset of the target from the beginning of its section.
        /// This is used to support debugging information and static thread
        /// local storage.
        /// </summary>
        IMAGE_REL_SH3_SECREL = 0x000F,

        /// <summary>
        /// The 32-bit RVA of the target symbol.
        /// </summary>
        IMAGE_REL_SH3_DIRECT32_NB = 0x0010,

        /// <summary>
        /// GP relative.
        /// </summary>
        IMAGE_REL_SH3_GPREL4_LONG = 0x0011,

        /// <summary>
        /// CLR token.
        /// </summary>
        IMAGE_REL_SH3_TOKEN = 0x0012,

        /// <summary>
        /// The offset from the current instruction in longwords. If the NOMODE
        /// bit is not set, insert the inverse of the low bit at bit 32 to
        /// select PTA or PTB.
        /// </summary>
        IMAGE_REL_SHM_PCRELPT = 0x0013,

        /// <summary>
        /// The low 16 bits of the 32-bit address.
        /// </summary>
        IMAGE_REL_SHM_REFLO = 0x0014,

        /// <summary>
        /// The high 16 bits of the 32-bit address.
        /// </summary>
        IMAGE_REL_SHM_REFHALF = 0x0015,

        /// <summary>
        /// The low 16 bits of the relative address.
        /// </summary>
        IMAGE_REL_SHM_RELLO = 0x0016,

        /// <summary>
        /// The high 16 bits of the relative address.
        /// </summary>
        IMAGE_REL_SHM_RELHALF = 0x0017,

        /// <summary>
        /// The relocation is valid only when it immediately follows a REFHALF,
        /// RELHALF, or RELLO relocation. The SymbolTableIndex field of the
        /// relocation contains a displacement and not an index into the symbol table.
        /// </summary>
        IMAGE_REL_SHM_PAIR = 0x0018,

        /// <summary>
        /// The relocation ignores section mode.
        /// </summary>
        IMAGE_REL_SHM_NOMODE = 0x8000,

        #endregion

        #region IBM PowerPC Processors

        /// <summary>
        /// The relocation is ignored.
        /// </summary>
        IMAGE_REL_PPC_ABSOLUTE = 0x0000,

        /// <summary>
        /// The 64-bit VA of the target.
        /// </summary>
        IMAGE_REL_PPC_ADDR64 = 0x0001,

        /// <summary>
        /// The 32-bit VA of the target.
        /// </summary>
        IMAGE_REL_PPC_ADDR32 = 0x0002,

        /// <summary>
        /// The low 24 bits of the VA of the target. This is valid only when
        /// the target symbol is absolute and can be sign-extended to its
        /// original value.
        /// </summary>
        IMAGE_REL_PPC_ADDR24 = 0x0003,

        /// <summary>
        /// The low 16 bits of the target's VA.
        /// </summary>
        IMAGE_REL_PPC_ADDR16 = 0x0004,

        /// <summary>
        /// The low 14 bits of the target's VA. This is valid only when the
        /// target symbol is absolute and can be sign-extended to its original
        /// value.
        /// </summary>
        IMAGE_REL_PPC_ADDR14 = 0x0005,

        /// <summary>
        /// A 24-bit PC-relative offset to the symbol's location.
        /// </summary>
        IMAGE_REL_PPC_REL24 = 0x0006,

        /// <summary>
        /// A 14-bit PC-relative offset to the symbol's location.
        /// </summary>
        IMAGE_REL_PPC_REL14 = 0x0007,

        /// <summary>
        /// The 32-bit RVA of the target.
        /// </summary>
        IMAGE_REL_PPC_ADDR32NB = 0x000A,

        /// <summary>
        /// The 32-bit offset of the target from the beginning of its section.
        /// This is used to support debugging information and static thread
        /// local storage.
        /// </summary>
        IMAGE_REL_PPC_SECREL = 0x000B,

        /// <summary>
        /// The 16-bit section index of the section that contains the target.
        /// This is used to support debugging information.
        /// </summary>
        IMAGE_REL_PPC_SECTION = 0x000C,

        /// <summary>
        /// The 16-bit offset of the target from the beginning of its section.
        /// This is used to support debugging information and static thread
        /// local storage.
        /// </summary>
        IMAGE_REL_PPC_SECREL16 = 0x000F,

        /// <summary>
        /// The high 16 bits of the target's 32-bit VA. This is used for the
        /// first instruction in a two-instruction sequence that loads a full
        /// address. This relocation must be immediately followed by a PAIR
        /// relocation whose SymbolTableIndex contains a signed 16-bit
        /// displacement that is added to the upper 16 bits that was taken
        /// from the location that is being relocated.
        /// </summary>
        IMAGE_REL_PPC_REFHI = 0x0010,

        /// <summary>
        /// The low 16 bits of the target's VA.
        /// </summary>
        IMAGE_REL_PPC_REFLO = 0x0011,

        /// <summary>
        /// A relocation that is valid only when it immediately follows a REFHI
        /// or SECRELHI relocation. Its SymbolTableIndex contains a displacement
        /// and not an index into the symbol table.
        /// </summary>
        IMAGE_REL_PPC_PAIR = 0x0012,

        /// <summary>
        /// The low 16 bits of the 32-bit offset of the target from the beginning
        /// of its section.
        /// </summary>
        IMAGE_REL_PPC_SECRELLO = 0x0013,

        /// <summary>
        /// The 16-bit signed displacement of the target relative to the GP register.
        /// </summary>
        IMAGE_REL_PPC_GPREL = 0x0015,

        /// <summary>
        /// The CLR token.
        /// </summary>
        IMAGE_REL_PPC_TOKEN = 0x0016,

        #endregion

        #region Intel 386 Processors

        /// <summary>
        /// The relocation is ignored.
        /// </summary>
        IMAGE_REL_I386_ABSOLUTE = 0x0000,

        /// <summary>
        /// Not supported.
        /// </summary>
        IMAGE_REL_I386_DIR16 = 0x0001,

        /// <summary>
        /// Not supported.
        /// </summary>
        IMAGE_REL_I386_REL16 = 0x0002,

        /// <summary>
        /// The target's 32-bit VA.
        /// </summary>
        IMAGE_REL_I386_DIR32 = 0x0006,

        /// <summary>
        /// The target's 32-bit RVA.
        /// </summary>
        IMAGE_REL_I386_DIR32NB = 0x0007,

        /// <summary>
        /// Not supported.
        /// </summary>
        IMAGE_REL_I386_SEG12 = 0x0009,

        /// <summary>
        /// The 16-bit section index of the section that contains the target.
        /// This is used to support debugging information.
        /// </summary>
        IMAGE_REL_I386_SECTION = 0x000A,

        /// <summary>
        /// The 32-bit offset of the target from the beginning of its section.
        /// This is used to support debugging information and static thread
        /// local storage.
        /// </summary>
        IMAGE_REL_I386_SECREL = 0x000B,

        /// <summary>
        /// The CLR token.
        /// </summary>
        IMAGE_REL_I386_TOKEN = 0x000C,

        /// <summary>
        /// A 7-bit offset from the base of the section that contains the target.
        /// </summary>
        IMAGE_REL_I386_SECREL7 = 0x000D,

        /// <summary>
        /// The 32-bit relative displacement to the target.This supports the x86 relative branch and call instructions.
        /// </summary>
        IMAGE_REL_I386_REL32 = 0x0014,

        #endregion

        #region Intel Itanium Processor Family (IPF)

        /// <summary>
        /// The relocation is ignored.
        /// </summary>
        IMAGE_REL_IA64_ABSOLUTE = 0x0000,

        /// <summary>
        /// The instruction relocation can be followed by an ADDEND relocation whose value is
        /// added to the target address before it is inserted into the specified slot in the
        /// IMM14 bundle. The relocation target must be absolute or the image must be fixed.
        /// </summary>
        IMAGE_REL_IA64_IMM14 = 0x0001,

        /// <summary>
        /// The instruction relocation can be followed by an ADDEND relocation whose value is
        /// added to the target address before it is inserted into the specified slot in the
        /// IMM22 bundle. The relocation target must be absolute or the image must be fixed.
        /// </summary>
        IMAGE_REL_IA64_IMM22 = 0x0002,

        /// <summary>
        /// The slot number of this relocation must be one (1). The relocation can be followed
        /// by an ADDEND relocation whose value is added to the target address before it is
        /// stored in all three slots of the IMM64 bundle.
        /// </summary>
        IMAGE_REL_IA64_IMM64 = 0x0003,

        /// <summary>
        /// The target's 32-bit VA. This is supported only for /LARGEADDRESSAWARE:NO images.
        /// </summary>
        IMAGE_REL_IA64_DIR32 = 0x0004,

        /// <summary>
        /// The target's 64-bit VA.
        /// </summary>
        IMAGE_REL_IA64_DIR64 = 0x0005,

        /// <summary>
        /// The instruction is fixed up with the 25-bit relative displacement to the 16-bit
        /// aligned target. The low 4 bits of the displacement are zero and are not stored.
        /// </summary>
        IMAGE_REL_IA64_PCREL21B = 0x0006,

        /// <summary>
        /// The instruction is fixed up with the 25-bit relative displacement to the 16-bit
        /// aligned target. The low 4 bits of the displacement, which are zero, are not stored.
        /// </summary>
        IMAGE_REL_IA64_PCREL21M = 0x0007,

        /// <summary>
        /// The LSBs of this relocation's offset must contain the slot number whereas the rest
        /// is the bundle address. The bundle is fixed up with the 25-bit relative displacement
        /// to the 16-bit aligned target. The low 4 bits of the displacement are zero and are
        /// not stored.
        /// </summary>
        IMAGE_REL_IA64_PCREL21F = 0x0008,

        /// <summary>
        /// The instruction relocation can be followed by an ADDEND relocation whose value is
        /// added to the target address and then a 22-bit GP-relative offset that is calculated
        /// and applied to the GPREL22 bundle.
        /// </summary>
        IMAGE_REL_IA64_GPREL22 = 0x0009,

        /// <summary>
        /// The instruction is fixed up with the 22-bit GP-relative offset to the target symbol's
        /// literal table entry. The linker creates this literal table entry based on this
        /// relocation and the ADDEND relocation that might follow.
        /// </summary>
        IMAGE_REL_IA64_LTOFF22 = 0x000A,

        /// <summary>
        /// The 16-bit section index of the section contains the target. This is used to support
        /// debugging information.
        /// </summary>
        IMAGE_REL_IA64_SECTION = 0x000B,

        /// <summary>
        /// The instruction is fixed up with the 22-bit offset of the target from the beginning of
        /// its section.This relocation can be followed immediately by an ADDEND relocation,
        /// whose Value field contains the 32-bit unsigned offset of the target from the beginning
        /// of the section.
        /// </summary>
        IMAGE_REL_IA64_SECREL22 = 0x000C,

        /// <summary>
        /// The slot number for this relocation must be one (1). The instruction is fixed up with
        /// the 64-bit offset of the target from the beginning of its section. This relocation can
        /// be followed immediately by an ADDEND relocation whose Value field contains the 32-bit
        /// unsigned offset of the target from the beginning of the section.
        /// </summary>
        IMAGE_REL_IA64_SECREL64I = 0x000D,

        /// <summary>
        /// The address of data to be fixed up with the 32-bit offset of the target from the beginning
        /// of its section.
        /// </summary>
        IMAGE_REL_IA64_SECREL32 = 0x000E,

        /// <summary>
        /// The target's 32-bit RVA.
        /// </summary>
        IMAGE_REL_IA64_DIR32NB = 0x0010,

        /// <summary>
        /// This is applied to a signed 14-bit immediate that contains the difference between two
        /// relocatable targets. This is a declarative field for the linker that indicates that the
        /// compiler has already emitted this value.
        /// </summary>
        IMAGE_REL_IA64_SREL14 = 0x0011,

        /// <summary>
        /// This is applied to a signed 22-bit immediate that contains the difference between two
        /// relocatable targets. This is a declarative field for the linker that indicates that the
        /// compiler has already emitted this value.
        /// </summary>
        IMAGE_REL_IA64_SREL22 = 0x0012,

        /// <summary>
        /// This is applied to a signed 32-bit immediate that contains the difference between two
        /// relocatable values.This is a declarative field for the linker that indicates that the
        /// compiler has already emitted this value.
        /// </summary>
        IMAGE_REL_IA64_SREL32 = 0x0013,

        /// <summary>
        /// This is applied to an unsigned 32-bit immediate that contains the difference between
        /// two relocatable values. This is a declarative field for the linker that indicates that
        /// the compiler has already emitted this value.
        /// </summary>
        IMAGE_REL_IA64_UREL32 = 0x0014,

        /// <summary>
        /// A 60-bit PC-relative fixup that always stays as a BRL instruction of an MLX bundle.
        /// </summary>
        IMAGE_REL_IA64_PCREL60X = 0x0015,

        /// <summary>
        /// A 60-bit PC-relative fixup. If the target displacement fits in a signed 25-bit field,
        /// convert the entire bundle to an MBB bundle with NOP. B in slot 1 and a 25-bit BR
        /// instruction (with the 4 lowest bits all zero and dropped) in slot 2.
        /// </summary>
        IMAGE_REL_IA64_PCREL60B = 0x0016,

        /// <summary>
        /// A 60-bit PC-relative fixup. If the target displacement fits in a signed 25-bit field,
        /// convert the entire bundle to an MFB bundle with NOP. F in slot 1 and a 25-bit
        /// (4 lowest bits all zero and dropped) BR instruction in slot 2.
        /// </summary>
        IMAGE_REL_IA64_PCREL60F = 0x0017,

        /// <summary>
        /// A 60-bit PC-relative fixup. If the target displacement fits in a signed 25-bit field,
        /// convert the entire bundle to an MIB bundle with NOP. I in slot 1 and a 25-bit
        /// (4 lowest bits all zero and dropped) BR instruction in slot 2.
        /// </summary>
        IMAGE_REL_IA64_PCREL60I = 0x0018,

        /// <summary>
        /// A 60-bit PC-relative fixup. If the target displacement fits in a signed 25-bit field,
        /// convert the entire bundle to an MMB bundle with NOP. M in slot 1 and a 25-bit
        /// (4 lowest bits all zero and dropped) BR instruction in slot 2.
        /// </summary>
        IMAGE_REL_IA64_PCREL60M = 0x0019,

        /// <summary>
        /// A 64-bit GP-relative fixup.
        /// </summary>
        IMAGE_REL_IA64_IMMGPREL64 = 0x001a,

        /// <summary>
        /// A CLR token.
        /// </summary>
        IMAGE_REL_IA64_TOKEN = 0x001b,

        /// <summary>
        /// A 32-bit GP-relative fixup.
        /// </summary>
        IMAGE_REL_IA64_GPREL32 = 0x001c,

        /// <summary>
        /// The relocation is valid only when it immediately follows one of the following relocations:
        /// IMM14, IMM22, IMM64, GPREL22, LTOFF22, LTOFF64, SECREL22, SECREL64I, or SECREL32.
        /// Its value contains the addend to apply to instructions within a bundle, not for data.
        /// </summary>
        IMAGE_REL_IA64_ADDEND = 0x001F,

        #endregion

        #region MIPS Processors

        /// <summary>
        /// The relocation is ignored.
        /// </summary>
        IMAGE_REL_MIPS_ABSOLUTE = 0x0000,

        /// <summary>
        /// The high 16 bits of the target's 32-bit VA.
        /// </summary>
        IMAGE_REL_MIPS_REFHALF = 0x0001,

        /// <summary>
        /// The target's 32-bit VA.
        /// </summary>
        IMAGE_REL_MIPS_REFWORD = 0x0002,

        /// <summary>
        /// The low 26 bits of the target's VA. This supports the MIPS J and JAL instructions.
        /// </summary>
        IMAGE_REL_MIPS_JMPADDR = 0x0003,

        /// <summary>
        /// The high 16 bits of the target's 32-bit VA. This is used for the first instruction in a
        /// two-instruction sequence that loads a full address. This relocation must be immediately
        /// followed by a PAIR relocation whose SymbolTableIndex contains a signed 16-bit displacement
        /// that is added to the upper 16 bits that are taken from the location that is being relocated.
        /// </summary>
        IMAGE_REL_MIPS_REFHI = 0x0004,

        /// <summary>
        /// The low 16 bits of the target's VA.
        /// </summary>
        IMAGE_REL_MIPS_REFLO = 0x0005,

        /// <summary>
        /// A 16-bit signed displacement of the target relative to the GP register.
        /// </summary>
        IMAGE_REL_MIPS_GPREL = 0x0006,

        /// <summary>
        /// The same as IMAGE_REL_MIPS_GPREL.
        /// </summary>
        IMAGE_REL_MIPS_LITERAL = 0x0007,

        /// <summary>
        /// The 16-bit section index of the section contains the target.
        /// This is used to support debugging information.
        /// </summary>
        IMAGE_REL_MIPS_SECTION = 0x000A,

        /// <summary>
        /// The 32-bit offset of the target from the beginning of its section.
        /// This is used to support debugging information and static thread local storage.
        /// </summary>
        IMAGE_REL_MIPS_SECREL = 0x000B,

        /// <summary>
        /// The low 16 bits of the 32-bit offset of the target from the beginning of its section.
        /// </summary>
        IMAGE_REL_MIPS_SECRELLO = 0x000C,

        /// <summary>
        /// The high 16 bits of the 32-bit offset of the target from the beginning of its section.
        /// An IMAGE_REL_MIPS_PAIR relocation must immediately follow this one. The SymbolTableIndex
        /// of the PAIR relocation contains a signed 16-bit displacement that is added to the upper
        /// 16 bits that are taken from the location that is being relocated.
        /// </summary>
        IMAGE_REL_MIPS_SECRELHI = 0x000D,

        /// <summary>
        /// The low 26 bits of the target's VA. This supports the MIPS16 JAL instruction.
        /// </summary>
        IMAGE_REL_MIPS_JMPADDR16 = 0x0010,

        /// <summary>
        /// The target's 32-bit RVA.
        /// </summary>
        IMAGE_REL_MIPS_REFWORDNB = 0x0022,

        /// <summary>
        /// The relocation is valid only when it immediately follows a REFHI or SECRELHI relocation.
        /// Its SymbolTableIndex contains a displacement and not an index into the symbol table.
        /// </summary>
        IMAGE_REL_MIPS_PAIR = 0x0025,

        #endregion

        #region Mitsubishi M32R

        /// <summary>
        /// The relocation is ignored.
        /// </summary>
        IMAGE_REL_M32R_ABSOLUTE = 0x0000,

        /// <summary>
        /// The target's 32-bit VA.
        /// </summary>
        IMAGE_REL_M32R_ADDR32 = 0x0001,

        /// <summary>
        /// The target's 32-bit RVA.
        /// </summary>
        IMAGE_REL_M32R_ADDR32NB = 0x0002,

        /// <summary>
        /// The target's 24-bit VA.
        /// </summary>
        IMAGE_REL_M32R_ADDR24 = 0x0003,

        /// <summary>
        /// The target's 16-bit offset from the GP register.
        /// </summary>
        IMAGE_REL_M32R_GPREL16 = 0x0004,

        /// <summary>
        /// The target's 24-bit offset from the program counter (PC), shifted left by
        /// 2 bits and sign-extended
        /// </summary>
        IMAGE_REL_M32R_PCREL24 = 0x0005,

        /// <summary>
        /// The target's 16-bit offset from the PC, shifted left by 2 bits and
        /// sign-extended
        /// </summary>
        IMAGE_REL_M32R_PCREL16 = 0x0006,

        /// <summary>
        /// The target's 8-bit offset from the PC, shifted left by 2 bits and
        /// sign-extended
        /// </summary>
        IMAGE_REL_M32R_PCREL8 = 0x0007,

        /// <summary>
        /// The 16 MSBs of the target VA.
        /// </summary>
        IMAGE_REL_M32R_REFHALF = 0x0008,

        /// <summary>
        /// The 16 MSBs of the target VA, adjusted for LSB sign extension. This is used for
        /// the first instruction in a two-instruction sequence that loads a full 32-bit address.
        /// This relocation must be immediately followed by a PAIR relocation whose SymbolTableIndex
        /// contains a signed 16-bit displacement that is added to the upper 16 bits that are
        /// taken from the location that is being relocated.
        /// </summary>
        IMAGE_REL_M32R_REFHI = 0x0009,

        /// <summary>
        /// The 16 LSBs of the target VA.
        /// </summary>
        IMAGE_REL_M32R_REFLO = 0x000A,

        /// <summary>
        /// The relocation must follow the REFHI relocation.Its SymbolTableIndex contains a displacement
        /// and not an index into the symbol table.
        /// </summary>
        IMAGE_REL_M32R_PAIR = 0x000B,

        /// <summary>
        /// The 16-bit section index of the section that contains the target. This is used to support
        /// debugging information.
        /// </summary>
        IMAGE_REL_M32R_SECTION = 0x000C,

        /// <summary>
        /// The 32-bit offset of the target from the beginning of its section.This is used to support
        /// debugging information and static thread local storage.
        /// </summary>
        IMAGE_REL_M32R_SECREL = 0x000D,

        /// <summary>
        /// The CLR token.
        /// </summary>
        IMAGE_REL_M32R_TOKEN = 0x000E,

        #endregion
    }

    [Flags]
    public enum SectionFlags : uint
    {
        /// <summary>
        /// Reserved for future use.
        /// </summary>
        RESERVED0 = 0x00000001,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        RESERVED1 = 0x00000002,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        RESERVED2 = 0x00000004,

        /// <summary>
        /// The section should not be padded to the next boundary.
        /// This flag is obsolete and is replaced by IMAGE_SCN_ALIGN_1BYTES.
        /// This is valid only for object files.
        /// </summary>
        IMAGE_SCN_TYPE_NO_PAD = 0x00000008,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        RESERVED4 = 0x00000010,

        /// <summary>
        /// The section contains executable code.
        /// </summary>
        IMAGE_SCN_CNT_CODE = 0x00000020,

        /// <summary>
        /// The section contains initialized data.
        /// </summary>
        IMAGE_SCN_CNT_INITIALIZED_DATA = 0x00000040,

        /// <summary>
        /// The section contains uninitialized data.
        /// </summary>
        IMAGE_SCN_CNT_UNINITIALIZED_DATA = 0x00000080,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        IMAGE_SCN_LNK_OTHER = 0x00000100,

        /// <summary>
        /// The section contains comments or other information. The .drectve
        /// section has this type. This is valid for object files only.
        /// </summary>
        IMAGE_SCN_LNK_INFO = 0x00000200,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        RESERVED10 = 0x00000400,

        /// <summary>
        /// The section will not become part of the image. This is valid
        /// only for object files.
        /// </summary>
        IMAGE_SCN_LNK_REMOVE = 0x00000800,

        /// <summary>
        /// The section contains COMDAT data. For more information, see COMDAT Sections
        /// (Object Only). This is valid only for object files.
        /// </summary>
        IMAGE_SCN_LNK_COMDAT = 0x00001000,

        /// <summary>
        /// The section contains data referenced through the global pointer (GP).
        /// </summary>
        IMAGE_SCN_GPREL = 0x00008000,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        IMAGE_SCN_MEM_PURGEABLE = 0x00010000,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        IMAGE_SCN_MEM_16BIT = 0x00020000,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        IMAGE_SCN_MEM_LOCKED = 0x00040000,

        /// <summary>
        /// Reserved for future use.
        /// </summary>
        IMAGE_SCN_MEM_PRELOAD = 0x00080000,

        /// <summary>
        /// Align data on a 1-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_1BYTES = 0x00100000,

        /// <summary>
        /// Align data on a 2-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_2BYTES = 0x00200000,

        /// <summary>
        /// Align data on a 4-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_4BYTES = 0x00300000,

        /// <summary>
        /// Align data on an 8-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_8BYTES = 0x00400000,

        /// <summary>
        /// Align data on a 16-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_16BYTES = 0x00500000,

        /// <summary>
        /// Align data on a 32-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_32BYTES = 0x00600000,

        /// <summary>
        /// Align data on a 64-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_64BYTES = 0x00700000,

        /// <summary>
        /// Align data on a 128-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_128BYTES = 0x00800000,

        /// <summary>
        /// Align data on a 256-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_256BYTES = 0x00900000,

        /// <summary>
        /// Align data on a 512-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_512BYTES = 0x00A00000,

        /// <summary>
        /// Align data on a 1024-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_1024BYTES = 0x00B00000,

        /// <summary>
        /// Align data on a 2048-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_2048BYTES = 0x00C00000,

        /// <summary>
        /// Align data on a 4096-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_4096BYTES = 0x00D00000,

        /// <summary>
        /// Align data on an 8192-byte boundary. Valid only for object files.
        /// </summary>
        IMAGE_SCN_ALIGN_8192BYTES = 0x00E00000,

        /// <summary>
        /// The section contains extended relocations.
        /// </summary>
        IMAGE_SCN_LNK_NRELOC_OVFL = 0x01000000,

        /// <summary>
        /// The section can be discarded as needed.
        /// </summary>
        IMAGE_SCN_MEM_DISCARDABLE = 0x02000000,

        /// <summary>
        /// The section cannot be cached.
        /// </summary>
        IMAGE_SCN_MEM_NOT_CACHED = 0x04000000,

        /// <summary>
        /// The section is not pageable.
        /// </summary>
        IMAGE_SCN_MEM_NOT_PAGED = 0x08000000,

        /// <summary>
        /// The section can be shared in memory.
        /// </summary>
        IMAGE_SCN_MEM_SHARED = 0x10000000,

        /// <summary>
        /// The section can be executed as code.
        /// </summary>
        IMAGE_SCN_MEM_EXECUTE = 0x20000000,

        /// <summary>
        /// The section can be read.
        /// </summary>
        IMAGE_SCN_MEM_READ = 0x40000000,

        /// <summary>
        /// The section can be written to.
        /// </summary>
        IMAGE_SCN_MEM_WRITE = 0x80000000,
    }

    public enum SectionNumber : short
    {
        /// <summary>
        /// The symbol record is not yet assigned a section. A value of
        /// zero indicates that a reference to an external symbol is
        /// defined elsewhere. A value of non-zero is a common symbol
        /// with a size that is specified by the value.
        /// </summary>
        IMAGE_SYM_UNDEFINED = 0,

        /// <summary>
        /// The symbol has an absolute (non-relocatable) value and
        /// is not an address.
        /// </summary>
        IMAGE_SYM_ABSOLUTE = -1,

        /// <summary>
        /// The symbol provides general type or debugging information
        /// but does not correspond to a section. Microsoft tools use
        /// this setting along with .file records (storage class FILE).
        /// </summary>
        IMAGE_SYM_DEBUG = -2,
    }

    public enum StorageClass : byte
    {
        /// <summary>
        /// A special symbol that represents the end of function, for debugging purposes.
        /// </summary>
        IMAGE_SYM_CLASS_END_OF_FUNCTION = 0xFF,

        /// <summary>
        /// No assigned storage class.
        /// </summary>
        IMAGE_SYM_CLASS_NULL = 0x00,

        /// <summary>
        /// The automatic (stack) variable.The Value field specifies the stack frame offset.
        /// </summary>
        IMAGE_SYM_CLASS_AUTOMATIC = 0x01,

        /// <summary>
        /// A value that Microsoft tools use for external symbols. The Value field indicates
        /// the size if the section number is IMAGE_SYM_UNDEFINED (0). If the section number
        /// is not zero, then the Value field specifies the offset within the section.
        /// </summary>
        IMAGE_SYM_CLASS_EXTERNAL = 0x02,

        /// <summary>
        /// The offset of the symbol within the section. If the Value field is zero, then
        /// the symbol represents a section name.
        /// </summary>
        IMAGE_SYM_CLASS_STATIC = 0x03,

        /// <summary>
        /// A register variable.The Value field specifies the register number.
        /// </summary>
        IMAGE_SYM_CLASS_REGISTER = 0x04,

        /// <summary>
        /// A symbol that is defined externally.
        /// </summary>
        IMAGE_SYM_CLASS_EXTERNAL_DEF = 0x05,

        /// <summary>
        /// A code label that is defined within the module. The Value field specifies the
        /// offset of the symbol within the section.
        /// </summary>
        IMAGE_SYM_CLASS_LABEL = 0x06,

        /// <summary>
        /// A reference to a code label that is not defined.
        /// </summary>
        IMAGE_SYM_CLASS_UNDEFINED_LABEL = 0x07,

        /// <summary>
        /// The structure member. The Value field specifies the n th member.
        /// </summary>
        IMAGE_SYM_CLASS_MEMBER_OF_STRUCT = 0x08,

        /// <summary>
        /// A formal argument (parameter) of a function. The Value field specifies the
        /// n th argument.
        /// </summary>
        IMAGE_SYM_CLASS_ARGUMENT = 0x09,

        /// <summary>
        /// The structure tag-name entry.
        /// </summary>
        IMAGE_SYM_CLASS_STRUCT_TAG = 0x0A,

        /// <summary>
        /// A union member. The Value field specifies the n th member.
        /// </summary>
        IMAGE_SYM_CLASS_MEMBER_OF_UNION = 0x0B,

        /// <summary>
        /// The Union tag-name entry.
        /// </summary>
        IMAGE_SYM_CLASS_UNION_TAG = 0x0C,

        /// <summary>
        /// A Typedef entry.
        /// </summary>
        IMAGE_SYM_CLASS_TYPE_DEFINITION = 0x0D,

        /// <summary>
        /// A static data declaration.
        /// </summary>
        IMAGE_SYM_CLASS_UNDEFINED_STATIC = 0x0E,

        /// <summary>
        /// An enumerated type tagname entry.
        /// </summary>
        IMAGE_SYM_CLASS_ENUM_TAG = 0x0F,

        /// <summary>
        /// A member of an enumeration. The Value field specifies the
        /// n th member.
        /// </summary>
        IMAGE_SYM_CLASS_MEMBER_OF_ENUM = 0x10,

        /// <summary>
        /// A register parameter.
        /// </summary>
        IMAGE_SYM_CLASS_REGISTER_PARAM = 0x11,

        /// <summary>
        /// A bit-field reference. The Value field specifies the
        /// n th bit in the bit field.
        /// </summary>
        IMAGE_SYM_CLASS_BIT_FIELD = 0x12,

        /// <summary>
        /// A .bb (beginning of block) or .eb (end of block) record.
        /// The Value field is the relocatable address of the code location.
        /// </summary>
        IMAGE_SYM_CLASS_BLOCK = 0x64,

        /// <summary>
        /// A value that Microsoft tools use for symbol records that define the extent
        /// of a function: begin function (.bf ), end function (.ef), and lines in
        /// function (.lf). For .lf records, the Value field gives the number of source
        /// lines in the function. For .ef records, the Value field gives the size of
        /// the function code.
        /// </summary>
        IMAGE_SYM_CLASS_FUNCTION = 0x65,

        /// <summary>
        /// An end-of-structure entry.
        /// </summary>
        IMAGE_SYM_CLASS_END_OF_STRUCT = 0x66,

        /// <summary>
        /// A value that Microsoft tools, as well as traditional COFF format, use for the
        /// source-file symbol record. The symbol is followed by auxiliary records that
        /// name the file.
        /// </summary>
        IMAGE_SYM_CLASS_FILE = 0x67,

        /// <summary>
        /// A definition of a section (Microsoft tools use STATIC storage class instead).
        /// </summary>
        IMAGE_SYM_CLASS_SECTION = 0x68,

        /// <summary>
        /// A weak external.For more information, see Auxiliary Format 3: Weak Externals.
        /// </summary>
        IMAGE_SYM_CLASS_WEAK_EXTERNAL = 0x69,

        /// <summary>
        /// A CLR token symbol. The name is an ASCII string that consists of the hexadecimal
        /// value of the token. For more information, see CLR Token Definition (Object Only).
        /// </summary>
        IMAGE_SYM_CLASS_CLR_TOKEN = 0x6A,
    }

    [Flags]
    public enum SymbolType : ushort
    {
        #region Simple (Base) Data Type

        /// <summary>
        /// No type information or unknown base type. Microsoft tools use this setting
        /// </summary>
        IMAGE_SYM_TYPE_NULL = 0x00,

        /// <summary>
        /// No valid type; used with void pointers and functions
        /// </summary>
        IMAGE_SYM_TYPE_VOID = 0x01,

        /// <summary>
        /// A character (signed byte)
        /// </summary>
        IMAGE_SYM_TYPE_CHAR = 0x02,

        /// <summary>
        /// A 2-byte signed integer
        /// </summary>
        IMAGE_SYM_TYPE_SHORT = 0x03,

        /// <summary>
        /// A natural integer type (normally 4 bytes in Windows)
        /// </summary>
        IMAGE_SYM_TYPE_INT = 0x04,

        /// <summary>
        /// A 4-byte signed integer
        /// </summary>
        IMAGE_SYM_TYPE_LONG = 0x05,

        /// <summary>
        /// A 4-byte floating-point number
        /// </summary>
        IMAGE_SYM_TYPE_FLOAT = 0x06,

        /// <summary>
        /// An 8-byte floating-point number
        /// </summary>
        IMAGE_SYM_TYPE_DOUBLE = 0x07,

        /// <summary>
        /// A structure
        /// </summary>
        IMAGE_SYM_TYPE_STRUCT = 0x08,

        /// <summary>
        /// A union
        /// </summary>
        IMAGE_SYM_TYPE_UNION = 0x09,

        /// <summary>
        /// An enumerated type
        /// </summary>
        IMAGE_SYM_TYPE_ENUM = 0x0A,

        /// <summary>
        /// A member of enumeration (a specific value)
        /// </summary>
        IMAGE_SYM_TYPE_MOE = 0x0B,

        /// <summary>
        /// A byte; unsigned 1-byte integer
        /// </summary>
        IMAGE_SYM_TYPE_BYTE = 0x0C,

        /// <summary>
        /// A word; unsigned 2-byte integer
        /// </summary>
        IMAGE_SYM_TYPE_WORD = 0x0D,

        /// <summary>
        /// An unsigned integer of natural size (normally, 4 bytes)
        /// </summary>
        IMAGE_SYM_TYPE_UINT = 0x0E,

        /// <summary>
        /// An unsigned 4-byte integer
        /// </summary>
        IMAGE_SYM_TYPE_DWORD = 0x0F,

        #endregion

        #region Complex Type

        /// <summary>
        /// The symbol is a pointer to base type
        /// </summary>
        IMAGE_SYM_DTYPE_POINTER = 0x10,

        /// <summary>
        /// The symbol is a function that returns a base type
        /// </summary>
        IMAGE_SYM_DTYPE_FUNCTION = 0x20,

        /// <summary>
        /// The symbol is an array of base type
        /// </summary>
        IMAGE_SYM_DTYPE_ARRAY = 0x30,

        #endregion
    }
}
