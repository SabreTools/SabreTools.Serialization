namespace SabreTools.Serialization.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class CubemapsLump : Lump
    {
        /// <summary>
        /// Cubemaps
        /// </summary>
        public Cubemap[]? Cubemaps { get; set; }
    }
}