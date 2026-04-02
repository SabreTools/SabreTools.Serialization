namespace SabreTools.Data.Models.ZArchive
{
    /// <summary>
    /// Offset and size values of a ZArchive section, stored in the Footer
    /// </summary>
    /// <see href="https://github.com/Exzap/ZArchive/"/>
    public class OffsetInfo
    {
        /// <summary>
        /// Base offset value for the section in bytes
        /// </summary>
		public ulong Offset { get; set; }

        /// <summary>
        /// Total size of the section in bytes
        /// </summary>
		public ulong Size { get; set; }
    }
}
