namespace SabreTools.Data.Models.WAD3
{
    /// <see href="https://twhl.info/wiki/page/Specification:_WAD3"/>
    public sealed class MipMap
    {
        /// <summary>
        /// Raw image data. Each byte points to an index in the palette
        /// </summary>
        /// <remarks>[width][height]</remarks>
        public byte[][] Data { get; set; }
    }
}
