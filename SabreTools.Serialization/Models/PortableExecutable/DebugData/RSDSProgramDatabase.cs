using System;
using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.PortableExecutable.DebugData
{
    /// <summary>
    /// This file describes the format of the pdb (Program Database) files of the "RSDS"
    /// or "DS" type which are emitted by Miscrosoft's link.exe from version 7 and above.
    /// </summary>
    /// <see href="http://www.godevtool.com/Other/pdb.htm"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class RSDSProgramDatabase
    {
        /// <summary>
        /// "RSDS" signature
        /// </summary>
        public uint Signature;

        /// <summary>
        /// 16-byte Globally Unique Identifier
        /// </summary>
        public Guid GUID;

        /// <summary>
        /// Ever-incrementing value, which is initially set to 1 and
        /// incremented every time when a part of the PDB file is updated
        /// without rewriting the whole file. 
        /// </summary>
        public uint Age;

        /// <summary>
        /// zero terminated UTF8 path and file name
        /// </summary>
#if NET472_OR_GREATER || NETCOREAPP
        [MarshalAs(UnmanagedType.LPUTF8Str)]
#else
        [MarshalAs(UnmanagedType.LPStr)]
#endif
        public string? PathAndFileName;
    }
}
