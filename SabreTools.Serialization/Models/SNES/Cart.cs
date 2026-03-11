namespace SabreTools.Data.Models.SNES
{
    /// <summary>
    /// SNES cart image
    /// </summary>
    /// <see href="https://www.raphnet.net/divers/documentation/Sneskart.txt"/>
    public class Cart
    {
        /// <summary>
        /// Copier header
        /// </summary>
        public Header? Header { get; set; }

        /// <summary>
        /// Cartridge data
        /// </summary>
        public byte[] Data { get; set; } = [];
    }
}
