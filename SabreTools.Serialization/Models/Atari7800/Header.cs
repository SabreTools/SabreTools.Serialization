namespace SabreTools.Data.Models.Atari7800
{
    /// <summary>
    /// Atari 7800 emulator header
    /// </summary>
    /// <see href="https://7800.8bitdev.org/index.php/A78_Header_Specification"/>
    public class Header
    {
        /// <summary>
        /// Header version
        /// </summary>
        /// <remarks>
        /// Current header version is 4.
        /// If no V4 features are used, this should be set to 3.
        /// </remarks>
        public byte HeaderVersion { get; set; }

        /// <summary>
        /// "ATARI7800" with 7 null bytes following
        /// </summary>
        public byte[] MagicText { get; set; } = new byte[16];

        /// <summary>
        /// Cart title, encoding not specified
        /// </summary>
        public byte[] CartTitle { get; set; } = new byte[32];

        /// <summary>
        /// ROM size without header data
        /// </summary>
        /// <remarks>
        /// Endianness possibly varies by version?
        /// Version 1 - Big-endian
        /// Version 2 - ?????
        /// Version 3 - Documented as little-endian (unconfirmed)
        /// Version 4 - Documented as little-endian (unconfirmed)
        /// </remarks>
        public uint RomSizeWithoutHeader { get; set; }

        /// <summary>
        /// Cart type flags
        /// </summary>
        public CartType CartType { get; set; }

        /// <summary>
        /// Controller 1 type
        /// </summary>
        public ControllerType Controller1Type { get; set; }

        /// <summary>
        /// Controller 2 type
        /// </summary>
        public ControllerType Controller2Type { get; set; }

        /// <summary>
        /// TV type
        /// </summary>
        public TVType TVType { get; set; }

        /// <summary>
        /// Save device
        /// </summary>
        public SaveDevice SaveDevice { get; set; }

        /// <summary>
        /// Reserved
        /// </summary>
        public byte[] Reserved { get; set; } = new byte[4];

        /// <summary>
        /// Slot passthrough device
        /// </summary>
        public SlotPassthroughDevice SlotPassthroughDevice { get; set; }

        #region Version 4+

        /// <summary>
        /// Mapper
        /// </summary>
        /// <remarks>v4+ only</remarks>
        public Mapper Mapper { get; set; }

        /// <summary>
        /// Mapper options
        /// </summary>
        /// <remarks>v4+ only</remarks>
        public MapperOptions MapperOptions { get; set; }

        /// <summary>
        /// Audio device(s)
        /// </summary>
        public AudioDevice AudioDevice { get; set; }

        /// <summary>
        /// Interrupt
        /// </summary>
        public Interrupt Interrupt { get; set; }

        #endregion

        /// <summary>
        /// Padding from end of header data to end magic text
        /// </summary>
        /// <remarks>
        /// 36 bytes in versions lower than 4.
        /// 30 bytes in versions 4 and above.
        /// </remarks>
        public byte[] Padding { get; set; } = [];

        /// <summary>
        /// "ACTUAL CART DATA STARTS HERE"
        /// </summary>
        public byte[] EndMagicText { get; set; } = new byte[28];
    }
}
