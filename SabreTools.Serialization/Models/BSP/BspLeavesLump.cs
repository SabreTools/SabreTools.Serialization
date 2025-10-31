namespace SabreTools.Data.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/>
    public sealed class BspLeavesLump : Lump
    {
        /// <summary>
        /// Leaves
        /// </summary>
        public BspLeaf[] Leaves { get; set; }
    }
}
