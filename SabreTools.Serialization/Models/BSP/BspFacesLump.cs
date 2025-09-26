namespace SabreTools.Serialization.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(GoldSrc)"/> 
    public sealed class BspFacesLump : Lump
    {
        /// <summary>
        /// Faces
        /// </summary>
        public BspFace[]? Faces { get; set; }
    }
}