namespace SabreTools.Data.Models.BZip2
{
    public class BlockTrees
    {
        // TODO: Implement SymMap

        /// <summary>
        /// Indicates the number of Huffman trees used in
        /// the HUFF stage. It must between 2 and 6.
        /// </summary>
        /// <remarks>Actually a 3-bit value</remarks>
        public byte NumTrees { get; set; }

        /// <summary>
        /// Indicates the number of selectors used in the
        /// HUFF stage. There must be at least 1 selector
        /// defined.
        /// </summary>
        /// <remarks>Actually a 15-bit value</remarks>
        public ushort NumSels { get; set; }

        // TODO: Implement Selectors

        // TODO: Implement Trees
    }
}
