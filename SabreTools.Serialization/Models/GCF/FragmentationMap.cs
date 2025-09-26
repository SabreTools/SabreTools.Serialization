using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.GCF
{
    /// <see href="https://github.com/RavuAlHemio/hllib/blob/master/HLLib/GCFFile.h"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class FragmentationMap
    {
        /// <summary>
        /// The index of the next data block.
        /// </summary>
        public uint NextDataBlockIndex;
    }
}