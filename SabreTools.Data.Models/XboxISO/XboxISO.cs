namespace SabreTools.Data.Models.XboxISO
{
    /// <summary>
    /// Xbox / Xbox 360 disc image with a video partition (ISO9660) and a game partition (XDVDFS)
    /// There exists zeroed gaps before/after the start/end of the game partition
    /// </summary>
    public class DiscImage
    {
        /// <summary>
        /// ISO9660 Video parititon, split across start and end of Disc Image
        /// </summary>
        public ISO9660.Volume VideoPartition { get; set; } = new();

        /// <summary>
        /// XGD type present in game partition
        /// 0 = XGD1
        /// 1 = XGD2
        /// 2 = XGD2-Hybrid
        /// 3 = XGD3
        /// </summary>
        /// <remarks>This field is not actually present, but is an abstract representation of the offset/length of the game partition</remarks>
        public int XGDType { get; set; }

        /// <summary>
        /// XDVDFS Game partition, present in middle of Disc Image
        /// </summary>
        public XDVDFS.Volume GamePartition { get; set; } = new();
    }
}
