namespace SabreTools.Serialization.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/> 
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class EntitiesLump : Lump
    {
        /// <summary>
        /// Entities
        /// </summary>
        public Entity[]? Entities { get; set; }
    }
}