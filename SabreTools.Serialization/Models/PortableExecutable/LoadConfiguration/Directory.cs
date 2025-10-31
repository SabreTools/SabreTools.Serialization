namespace SabreTools.Data.Models.PortableExecutable.LoadConfiguration
{
    /// <summary>
    /// The data directory entry for a pre-reserved SEH load configuration
    /// structure must specify a particular size of the load configuration
    /// structure because the operating system loader always expects it to
    /// be a certain value. In that regard, the size is really only a
    /// version check. For compatibility with Windows XP and earlier versions
    /// of Windows, the size must be 64 for x86 images.
    /// </summary>
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/debug/pe-format"/>
    public sealed class Directory
    {
        /// <summary>
        /// Flags that indicate attributes of the file, currently unused.
        /// </summary>
        public uint Characteristics { get; set; }

        /// <summary>
        /// Date and time stamp value. The value is represented in the number of
        /// seconds that have elapsed since midnight (00:00:00), January 1, 1970,
        /// Universal Coordinated Time, according to the system clock. The time
        /// stamp can be printed by using the C runtime (CRT) time function.
        /// </summary>
        public uint TimeDateStamp { get; set; }

        /// <summary>
        /// Major version number.
        /// </summary>
        public ushort MajorVersion { get; set; }

        /// <summary>
        /// Minor version number.
        /// </summary>
        public ushort MinorVersion { get; set; }

        /// <summary>
        /// The global loader flags to clear for this process as the loader starts
        /// the process.
        /// </summary>
        public uint GlobalFlagsClear { get; set; }

        /// <summary>
        /// The global loader flags to set for this process as the loader starts
        /// the process.
        /// </summary>
        public uint GlobalFlagsSet { get; set; }

        /// <summary>
        /// Memory that must be freed before it is returned to the system, in bytes.
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong DeCommitFreeBlockThreshold { get; set; }

        /// <summary>
        /// Total amount of free memory, in bytes.
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong DeCommitTotalFreeThreshold { get; set; }

        /// <summary>
        /// [x86 only] The VA of a list of addresses where the LOCK prefix is used so
        /// that they can be replaced with NOP on single processor machines.
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong LockPrefixTable { get; set; }

        /// <summary>
        /// Maximum allocation size, in bytes.
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong MaximumAllocationSize { get; set; }

        /// <summary>
        /// Maximum virtual memory size, in bytes.
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong VirtualMemoryThreshold { get; set; }

        /// <summary>
        /// Setting this field to a non-zero value is equivalent to calling
        /// SetProcessAffinityMask with this value during process startup (.exe only)
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong ProcessAffinityMask { get; set; }

        /// <summary>
        /// Process heap flags that correspond to the first argument of the
        /// HeapCreate function. These flags apply to the process heap that
        /// is created during process startup.
        /// </summary>
        public uint ProcessHeapFlags { get; set; }

        /// <summary>
        /// The service pack version identifier.
        /// </summary>
        public ushort CSDVersion { get; set; }

        /// <summary>
        /// Must be zero.
        /// </summary>
        public ushort Reserved { get; set; }

        /// <summary>
        /// Reserved for use by the system.
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong EditList { get; set; }

        // <summary>
        /// A pointer to a cookie that is used by Visual C++ or GS implementation.
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong SecurityCookie { get; set; }

        /// <summary>
        /// [x86 only] The VA of the sorted table of RVAs of each valid, unique
        /// SE handler in the image.
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong SEHandlerTable { get; set; }

        /// <summary>
        /// [x86 only] The count of unique handlers in the table.
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong SEHandlerCount { get; set; }

        /// <summary>
        /// The VA where Control Flow Guard check-function pointer is stored.
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong GuardCFCheckFunctionPointer { get; set; }

        /// <summary>
        /// The VA where Control Flow Guard dispatch-function pointer is stored.
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong GuardCFDispatchFunctionPointer { get; set; }

        /// <summary>
        /// The VA of the sorted table of RVAs of each Control Flow Guard
        /// function in the image.
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong GuardCFFunctionTable { get; set; }

        /// <summary>
        /// The count of unique RVAs in the above table.
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong GuardCFFunctionCount { get; set; }

        /// <summary>
        /// Control Flow Guard related flags.
        /// </summary>
        public GuardFlags GuardFlags { get; set; }

        /// <summary>
        /// Code integrity information.
        /// </summary>
        /// <remarks>12 bytes</remarks>
        public byte[] CodeIntegrity { get; set; } = new byte[12];

        /// <summary>
        /// The VA where Control Flow Guard address taken IAT table is stored.
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong GuardAddressTakenIatEntryTable { get; set; }

        /// <summary>
        /// The count of unique RVAs in the above table.
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong GuardAddressTakenIatEntryCount { get; set; }

        /// <summary>
        /// The VA where Control Flow Guard long jump target table is stored.
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong GuardLongJumpTargetTable { get; set; }

        /// <summary>
        /// The count of unique RVAs in the above table.
        /// </summary>
        /// <remarks>This value is 32-bit if PE32 and 64-bit if PE32+</remarks>
        public ulong GuardLongJumpTargetCount { get; set; }
    }
}
