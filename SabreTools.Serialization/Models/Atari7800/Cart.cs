namespace SabreTools.Data.Models.Atari7800
{
    /// <summary>
    /// Atari 7800 headered cart
    /// </summary>
    /// <see href="https://7800.8bitdev.org/index.php/A78_Header_Specification"/>
    public class Cart
    {
        /// <summary>
        /// A78 header
        /// </summary>
        /// <remarks>If omitted, format is technically BIN</remarks>
        public Header? Header { get; set; }

        /// <summary>
        /// Cartridge data
        /// </summary>
        public byte[] Data { get; set; } = [];
    }
}
