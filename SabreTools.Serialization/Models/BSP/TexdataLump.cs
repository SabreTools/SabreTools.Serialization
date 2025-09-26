namespace SabreTools.Serialization.Models.BSP
{
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class TexdataLump : Lump
    {
        /// <summary>
        /// Texdatas
        /// </summary>
        public Texdata[]? Texdatas { get; set; }
    }
}