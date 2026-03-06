namespace SabreTools.Data.Models.NES
{
    /// <summary>
    /// NES headered cart
    /// </summary>
    /// <see href="https://www.nesdev.org/wiki/INES"/>
    /// <see href="https://www.nesdev.org/wiki/NES_2.0"/>
    public class Cart
    {
        /// <summary>
        /// NES 1.0 or 2.0 header
        /// </summary>
        public Header? Header { get; set; }

        /// <summary>
        /// Trainer, if present (0 or 512 bytes)
        /// </summary>
        public byte[] Trainer { get; set; } = [];

        /// <summary>
        /// PRG ROM data (16384 * x bytes)
        /// </summary>
        public byte[] PRGROMData { get; set; } = [];

        /// <summary>
        /// CHR ROM data, if present (8192 * y bytes)
        /// </summary>
        public byte[] CHRROMData { get; set; } = [];

        /// <summary>
        /// PlayChoice INST-ROM, if present (0 or 8192 bytes)
        /// </summary>
        public byte[] PlayChoiceINSTROM { get; set; } = [];

        /// <summary>
        /// PlayChoice PROM, if present (16 bytes Data, 16 bytes CounterOut)
        /// </summary>
        /// <remarks>
        /// This is often missing; see PC10 ROM-Images for details
        /// 16 bytes RP5H01 PROM Data output (needed to decrypt the INST ROM)
        /// 16 bytes RP5H01 PROM CounterOut output (needed to decrypt the INST ROM) (usually constant: 00,00,00,00,FF,FF,FF,FF,00,00,00,00,FF,FF,FF,FF)
        /// </remarks>
        /// TODO: Split into 2 parts
        public byte[] PlayChoicePROM { get; set; } = [];

        /// <summary>
        /// Some ROM-Images additionally contain a 128-byte (or sometimes 127-byte)
        /// title at the end of the file.
        /// </summary>
        public byte[] Title { get; set; } = [];
    }
}
