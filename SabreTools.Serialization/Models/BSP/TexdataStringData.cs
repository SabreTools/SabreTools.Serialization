namespace SabreTools.Data.Models.BSP
{
    /// <summary>
    /// The TexdataStringData lump consists of concatenated null-terminated
    /// strings giving the texture name.
    /// </summary>
    /// <see href="https://developer.valvesoftware.com/wiki/BSP_(Source)"/>
    public sealed class TexdataStringData : Lump
    {
        /// <summary>
        /// Strings
        /// </summary>
        public string[] Strings { get; set; }
    }
}
