namespace SabreTools.Serialization.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/> 
    public sealed class VbspNodesLump : Lump
    {
        /// <summary>
        /// Nodes
        /// </summary>
        public VbspNode[]? Nodes { get; set; }
    }
}