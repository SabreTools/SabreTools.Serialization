namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// The VIS lump contains data, which is irrelevant to the actual
    /// BSP tree, but offers a way to boost up the speed of the
    /// renderer significantly. Especially complex maps profit from
    /// the use if this data. This lump contains the so-called
    /// Potentially Visible Sets (PVS) (also called VIS lists) in the
    /// same amout of leaves of the tree, the user can enter (often
    /// referred to as VisLeaves). The visiblilty lists are stored as
    /// sequences of bitfields, which are run-length encoded.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class VisibilityLump
    {
        public int NumClusters { get; set; }

        /// <remarks>[numclusters][2]</remarks>
        public int[][] ByteOffsets { get; set; } = [];
    }
}
