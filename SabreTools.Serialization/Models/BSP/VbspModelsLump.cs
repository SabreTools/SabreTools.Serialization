namespace SabreTools.Serialization.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/> 
    public sealed class VbspModelsLump : Lump
    {
        /// <summary>
        /// Model
        /// </summary>
        public VbspModel[]? Models { get; set; }
    }
}