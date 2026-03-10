namespace SabreTools.Data.Models.NES
{
    /// <summary>
    /// Quick Disk Famicom Disk System image
    /// </summary>
    /// <see href="https://www.nesdev.org/wiki/QD"/>
    public class QD
    {
        /// <summary>
        /// Disk data (65536 * x bytes)
        /// </summary>
        public byte[] Data { get; set; } = [];
    }
}
