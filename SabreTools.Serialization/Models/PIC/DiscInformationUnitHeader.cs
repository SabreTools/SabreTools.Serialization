using System.Runtime.InteropServices;

namespace SabreTools.Data.Models.PIC
{
    /// <see href="https://www.t10.org/ftp/t10/document.05/05-206r0.pdf"/>
    /// <see href="https://github.com/aaru-dps/Aaru.Decoders/blob/devel/Bluray/DI.cs"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class DiscInformationUnitHeader
    {
        /// <summary>
        /// Disc Information Identifier "DI"
        /// Emergency Brake Identifier "EB"
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string? DiscInformationIdentifier;

        /// <summary>
        /// Disc Information Format
        /// </summary>
        public byte DiscInformationFormat;

        /// <summary>
        /// Number of DI units in each DI block
        /// </summary>
        public byte NumberOfUnitsInBlock;

        /// <summary>
        /// Should be 0x00
        /// </summary>
        public byte Reserved0;

        /// <summary>
        /// DI unit Sequence Number
        /// </summary>
        public byte SequenceNumber;

        /// <summary>
        /// Number of bytes in use in this DI unit
        /// </summary>
        public byte BytesInUse;

        /// <summary>
        /// Should be 0x00
        /// </summary>
        public byte Reserved1;
    }
}
