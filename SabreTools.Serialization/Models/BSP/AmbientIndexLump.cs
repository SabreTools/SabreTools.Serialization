namespace SabreTools.Data.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class AmbientIndexLump : Lump
    {
        /// <summary>
        /// Indicies
        /// </summary>
        public LeafAmbientIndex[] Indicies { get; set; } = [];
    }
}
