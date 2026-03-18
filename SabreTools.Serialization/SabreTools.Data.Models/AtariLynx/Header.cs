namespace SabreTools.Data.Models.AtariLynx
{
    /// <summary>
    /// Atari Lynx emulator header
    /// </summary>
    /// <see href="https://github.com/mozzwald/handy-sdl/blob/master/src/handy-0.95/cart.h"/>
    public class Header
    {
        /// <summary>
        /// "LYNX"
        /// </summary>
        /// <remarks>4 bytes</remarks>
        public byte[] Magic { get; set; } = new byte[4];

        /// <summary>
        /// Page size for bank 0
        /// </summary>
        public ushort Bank0PageSize { get; set; }

        /// <summary>
        /// Page size for bank 1
        /// </summary>
        public ushort Bank1PageSize { get; set; }

        /// <summary>
        /// Header version
        /// </summary>
        /// <remarks>Must be 0x0001</remarks>
        public ushort Version { get; set; }

        /// <summary>
        /// Game title (Null-padded ASCII)
        /// </summary>
        /// <remarks>32 bytes</remarks>
        public byte[] CartName { get; set; } = new byte[32];

        /// <summary>
        /// Publisher name
        /// </summary>
        /// <remarks>16 bytes</remarks>
        public byte[] Manufacturer { get; set; } = new byte[16];

        /// <summary>
        /// Screen rotation
        /// </summary>
        public Rotation Rotation { get; set; }

        /// <summary>
        /// Padding
        /// </summary>
        /// <remarks>5 bytes</remarks>
        public byte[] Spare { get; set; } = new byte[5];
    }
}
