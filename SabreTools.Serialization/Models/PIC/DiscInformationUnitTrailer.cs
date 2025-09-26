using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.PIC
{
    /// <summary>
    /// BD-R/RE only
    /// </summary>
    /// <see href="https://www.t10.org/ftp/t10/document.05/05-206r0.pdf"/>
    /// <see href="https://github.com/aaru-dps/Aaru.Decoders/blob/devel/Bluray/DI.cs"/>
    [StructLayout(LayoutKind.Sequential)]
    public class DiscInformationUnitTrailer
    {
        /// <summary>
        /// Disc Manufacturer ID
        /// </summary>
        /// <remarks>6 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[]? DiscManufacturerID;

        /// <summary>
        /// Media Type ID
        /// </summary>
        /// <remarks>3 bytes</remarks>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[]? MediaTypeID;

        /// <summary>
        /// Time Stamp
        /// </summary>
        public ushort TimeStamp;

        /// <summary>
        /// Product Revision Number
        /// </summary>
        public byte ProductRevisionNumber;
    }
}
