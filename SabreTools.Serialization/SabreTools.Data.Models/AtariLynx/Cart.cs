namespace SabreTools.Data.Models.AtariLynx
{
    /// <summary>
    /// Atari Lynx headered cart (LNX)
    /// </summary>
    /// <see href="https://github.com/mozzwald/handy-sdl/blob/master/src/handy-0.95/cart.h"/>
    public class Cart
    {
        /// <summary>
        /// LNX header
        /// </summary>
        /// <remarks>If omitted, format is technically LYX</remarks>
        public Header? Header { get; set; }

        /// <summary>
        /// Cartridge data
        /// </summary>
        public byte[] Data { get; set; } = [];
    }
}
