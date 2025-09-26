using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.PKZIP
{
    /// <summary>
    /// Zip64 end of central directory locator
    /// </summary>
    /// <see href="https://pkware.cachefly.net/webdocs/casestudies/APPNOTE.TXT"/> 
    [StructLayout(LayoutKind.Sequential)]
    public class EndOfCentralDirectoryLocator64
    {
        /// <summary>
        /// ZIP64 end of central directory locator signature (0x07064B50)
        /// </summary>
        public uint Signature;

        /// <summary>
        /// Number of the disk with the start of the end of central directory
        /// </summary>
        public uint StartDiskNumber;

        /// <summary>
        /// Relative offset of start of central directory record
        /// </summary>
        public ulong CentralDirectoryOffset;

        /// <summary>
        /// Total number of disks
        /// </summary>
        public uint TotalDisks;
    }
}
