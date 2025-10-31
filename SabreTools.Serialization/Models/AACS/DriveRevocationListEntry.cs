using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.AACS
{
    /// <see href="https://aacsla.com/wp-content/uploads/2019/02/AACS_Spec_Common_Final_0953.pdf"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class DriveRevocationListEntry
    {
        /// <summary>
        /// A 2-byte Range value indicates the range of revoked ID’s starting
        /// from the ID contained in the record. A value of zero in the Range
        /// field indicates that only one ID is being revoked, a value of one
        /// in the Range field indicates two ID’s are being revoked, and so on.
        /// </summary>
        public ushort Range;

        /// <summary>
        /// A 6-byte Drive ID value identifying the Licensed Drive being revoked
        /// (or the first in a range of Licensed Drives being revoked, in the
        /// case of a non-zero Range value).
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] DriveID = new byte[6];
    }
}
