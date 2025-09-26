namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// Presumably, this array is used to index into the leaf lump
    /// to locate the leaves that each prop static is located in.
    /// Note that a prop static may span several leaves.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class StaticPropLeafLump
    {
        public int LeafEntries;

        /// <summary>
        /// <see cref="LeafEntries">
        /// </summary>
        public ushort[]? Leaf;
    }
}