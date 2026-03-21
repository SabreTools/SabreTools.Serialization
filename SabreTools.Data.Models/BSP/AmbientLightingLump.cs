namespace SabreTools.Data.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class AmbientLightingLump : Lump
    {
        /// <summary>
        /// Lightings
        /// </summary>
        public LeafAmbientLighting[] Lightings { get; set; } = [];
    }
}
