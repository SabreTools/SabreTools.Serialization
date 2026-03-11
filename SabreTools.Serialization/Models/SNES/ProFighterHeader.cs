namespace SabreTools.Data.Models.SNES
{
    /// <summary>
    /// Pro Fighter (FIG) header
    /// </summary>
    /// <see href="https://www.raphnet.net/divers/documentation/Sneskart.txt"/>
    public class ProFighterHeader : Header
    {
        /// <summary>
        /// Size word
        /// </summary>
        /// <remarks>Little-endian</remarks>
        public ushort Size { get; set; }

        /// <summary>
        /// Single- or multi-image
        /// </summary>
        public FIGImageMode ImageMode { get; set; }

        /// <summary>
        /// ROM mode
        /// </summary>
        public FIGRomMode RomMode { get; set; }

        /// <summary>
        /// First DSP1 SRAM flag
        /// </summary>
        public FIGDSPMode1 DSPMode1 { get; set; }

        /// <summary>
        /// Second DSP1 SRAM flag
        /// </summary>
        public FIGDSPMode2 DSPMode2 { get; set; }

        /// <summary>
        /// Padding to get to 512 bytes
        /// </summary>
        /// <remarks>506 bytes</remarks>
        public byte[] Padding { get; set; } = new byte[506];
    }
}
