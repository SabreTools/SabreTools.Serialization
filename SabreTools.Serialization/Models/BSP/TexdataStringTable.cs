namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// The TexdataStringTable (Lump 44) is an array of integers which
    /// are offsets into the TexdataStringData (lump 43).
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class TexdataStringTable : Lump
    {
        /// <summary>
        /// Offsets
        /// </summary>
        public int[]? Offsets { get; set; }
    }
}