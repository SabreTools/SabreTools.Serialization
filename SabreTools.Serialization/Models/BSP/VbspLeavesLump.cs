namespace SabreTools.Serialization.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/> 
    public sealed class VbspLeavesLump : Lump
    {
        /// <summary>
        /// Leaves
        /// </summary>
        public VbspLeaf[]? Leaves { get; set; }
    }
}