namespace SabreTools.Data.Models.XboxExecutable
{
    /// <see href="https://www.caustik.com/cxbx/download/xbe.htm"/>
    /// <see href="https://github.com/Cxbx-Reloaded/Cxbx-Reloaded/blob/master/src/common/xbe/Xbe.h"/>
    public static class Constants
    {
        /// <summary>
        /// XBox Executable magic number ("XBEH")
        /// </summary>
        public static readonly byte[] MagicBytes = [0x58, 0x42, 0x45, 0x48];

        /// <summary>
        /// XBox Executable magic number ("XBEH")
        /// </summary>
        public const string MagicString = "XBEH";

        /// <summary>
        /// XBox Executable magic number ("XBEH")
        /// </summary>
        public const uint MagicUInt32 = 0x48454258;
    }
}
