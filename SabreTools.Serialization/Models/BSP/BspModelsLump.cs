namespace SabreTools.Serialization.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/> 
    public sealed class BspModelsLump : Lump
    {
        /// <summary>
        /// Model
        /// </summary>
        public BspModel[]? Models { get; set; }
    }
}