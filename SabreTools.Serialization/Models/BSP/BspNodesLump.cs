namespace SabreTools.Data.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/>
    public sealed class BspNodesLump : Lump
    {
        /// <summary>
        /// Nodes
        /// </summary>
        public BspNode[] Nodes { get; set; } = [];
    }
}
