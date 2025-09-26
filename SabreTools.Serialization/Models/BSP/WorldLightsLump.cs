namespace SabreTools.Serialization.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/> 
    public sealed class WorldLightsLump : Lump
    {
        /// <summary>
        /// WorldLights
        /// </summary>
        public WorldLight[]? WorldLights { get; set; }
    }
}