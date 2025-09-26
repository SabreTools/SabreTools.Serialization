using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.TAR
{
    /// <see href="https://www.ibm.com/docs/en/aix/7.3?topic=files-tarh-file"/> 
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public sealed class Header
    {
        /// <summary>
        /// File name without a forward slash
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string? FileName;

        /// <summary>
        /// File mode
        /// </summary>
        /// <remarks>Octal string representation</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string? Mode;

        /// <summary>
        /// Owner's numeric user ID
        /// </summary>
        /// <remarks>Octal string representation</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string? UID;

        /// <summary>
        /// Owner's numeric group ID
        /// </summary>
        /// <remarks>Octal string representation</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string? GID;

        /// <summary>
        /// File size in bytes
        /// </summary>
        /// <remarks>Octal string representation</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string? Size;

        /// <summary>
        /// Last modification time in numeric Unix time format
        /// </summary>
        /// <remarks>Octal string representation</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string? ModifiedTime;

        /// <summary>
        /// Checksum for header record
        /// </summary>
        /// <remarks>Octal string representation</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string? Checksum;

        /// <summary>
        /// Link indicator (file type) / Type flag
        /// </summary>
        public TypeFlag TypeFlag;

        /// <summary>
        /// Linked path name or file name
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string? LinkName;

        #region USTAR Extension

        /// <summary>
        /// UStar indicator, "ustar"
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string? Magic;

        /// <summary>
        /// UStar version, "00"
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 3)]
        public string? Version;

        /// <summary>
        /// Owner user name
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string? UserName;

        /// <summary>
        /// Owner group name
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string? GroupName;

        /// <summary>
        /// Device major number
        /// </summary>
        /// <remarks>Octal string representation(?)</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string? DevMajor;

        /// <summary>
        /// Device minor number
        /// </summary>
        /// <remarks>Octal string representation(?)</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string? DevMinor;

        /// <summary>
        /// Path name without trailing slashes
        /// </summary>
        /// <remarks>155 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 155)]
        public string? Prefix;

        #endregion
    }
}