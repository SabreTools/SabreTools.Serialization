using System.Runtime.InteropServices;

namespace SabreTools.Serialization.Models.BSP
{
    /// <summary>
    /// To associate each leaf with its collection of ambient samples,
    /// the ambient lighting index lumps (Lumps 51 and 52) are used.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    [StructLayout(LayoutKind.Sequential)]
    public sealed class LeafAmbientIndex
    {
        public ushort AmbientSampleCount;

        public ushort FirstAmbientSample;
    }
}