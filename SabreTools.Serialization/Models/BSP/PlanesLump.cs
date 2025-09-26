namespace SabreTools.Data.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/> 
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class PlanesLump : Lump
    {
        /// <summary>
        /// Planes
        /// </summary>
        public Plane[]? Planes { get; set; }
    }
}