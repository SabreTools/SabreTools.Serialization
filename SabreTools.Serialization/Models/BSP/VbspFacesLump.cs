namespace SabreTools.Data.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class VbspFacesLump : Lump
    {
        /// <summary>
        /// Faces
        /// </summary>
        public VbspFace[]? Faces { get; set; }
    }
}