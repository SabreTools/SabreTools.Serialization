using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.N3DS
{
    /// <see href="https://www.3dbrew.org/wiki/NCCH/Extended_Header#Code_Set_Info"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class CodeSetInfo
    {
        /// <summary>
        /// Address
        /// </summary>
        public uint Address;

        /// <summary>
        /// Physical region size (in page-multiples)
        /// </summary>
        public uint PhysicalRegionSizeInPages;

        /// <summary>
        /// Size (in bytes)
        /// </summary>
        public uint SizeInBytes;
    }
}
