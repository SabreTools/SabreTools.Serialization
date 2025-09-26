using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.BDPlus
{
    /// <see href="https://github.com/mwgoldsmith/bdplus/blob/master/src/libbdplus/bdsvm/loader.c"/>
    public sealed class SVM
    {
        /// <summary>
        /// "BDSVM_CC"
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string? Signature;

        /// <summary>
        /// Unknown data
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public byte[]? Unknown1 = new byte[5];

        /// <summary>
        /// Version year
        /// </summary>
        public ushort Year;

        /// <summary>
        /// Version month
        /// </summary>
        public byte Month;

        /// <summary>
        /// Version day
        /// </summary>
        public byte Day;

        /// <summary>
        /// Unknown data
        /// </summary>
        public uint Unknown2;

        /// <summary>
        /// Length
        /// </summary>
        public uint Length;

        /// <summary>
        /// Length bytes of data
        /// </summary>
        public byte[]? Data { get; set; }
    }
}